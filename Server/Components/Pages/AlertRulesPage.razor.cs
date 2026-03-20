using CoreConnect.Shared.Entities;
using CoreConnect.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using CoreConnect.Server.Services;

namespace CoreConnect.Server.Components.Pages;

public partial class AlertRulesPage : ComponentBase
{
    [Inject]
    public IDataService DataService { get; set; } = null!;

    [Inject]
    public IToastService ToastService { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthStateTask { get; set; } = null!;

    public AlertRule[]? Rules { get; set; }
    public List<SavedScript> AvailableScripts { get; set; } = new();
    public List<DeviceGroup> AvailableGroups { get; set; } = new();

    public AlertRule EditingRule { get; set; } = new();
    public bool IsEditing { get; set; }

    private string _organizationId = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        var user = authState.User;
        
        var orgResult = await DataService.GetOrganizationByUserName(user.Identity?.Name ?? string.Empty);
        if (orgResult.IsSuccess)
        {
            _organizationId = orgResult.Value.ID;
            await LoadData();
        }
    }

    private async Task LoadData()
    {
        Rules = await DataService.GetAlertRules(_organizationId);
        
        var authState = await AuthStateTask;
        AvailableScripts = await DataService.GetSavedScriptsWithoutContent(
            authState.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty, 
            _organizationId);

        AvailableGroups = DataService.GetDeviceGroupsForOrganization(_organizationId).ToList();
            
        StateHasChanged();
    }

    private void CreateNewRule()
    {
        EditingRule = new AlertRule
        {
            ID = string.Empty,
            OrganizationID = _organizationId,
            Metric = "CpuUtilization",
            Operator = ">",
            Threshold = 90,
            IsEnabled = true
        };
        IsEditing = true;
    }

    private void EditRule(AlertRule rule)
    {
        EditingRule = new AlertRule
        {
            ID = rule.ID,
            Name = rule.Name,
            OrganizationID = rule.OrganizationID,
            Metric = rule.Metric,
            Operator = rule.Operator,
            Threshold = rule.Threshold,
            SavedScriptId = rule.SavedScriptId,
            TargetDeviceGroupId = rule.TargetDeviceGroupId,
            IsEnabled = rule.IsEnabled
        };
        IsEditing = true;
    }

    private async Task ToggleRule(AlertRule rule, bool isEnabled)
    {
        rule.IsEnabled = isEnabled;
        await DataService.UpdateAlertRule(rule);
        ToastService.ShowToast("Alert rule updated.", classString: "bg-success");
    }

    private async Task DeleteRule(AlertRule rule)
    {
        await DataService.DeleteAlertRule(rule.ID, _organizationId);
        ToastService.ShowToast("Alert rule deleted.", classString: "bg-success");
        await LoadData();
    }

    private async Task SaveRule()
    {
        if (string.IsNullOrWhiteSpace(EditingRule.Name))
        {
            ToastService.ShowToast("Rule name is required.", classString: "bg-warning");
            return;
        }

        if (string.IsNullOrEmpty(EditingRule.ID))
        {
            EditingRule.ID = Guid.NewGuid().ToString();
            await DataService.AddAlertRule(EditingRule);
            ToastService.ShowToast("Alert rule created.", classString: "bg-success");
        }
        else
        {
            await DataService.UpdateAlertRule(EditingRule);
            ToastService.ShowToast("Alert rule updated.", classString: "bg-success");
        }

        IsEditing = false;
        await LoadData();
    }

    private void CancelEdit()
    {
        IsEditing = false;
    }
}
