# Issue #885: High CPU, RAM and I/O usage

**State:** open
**Created At:** 2024-05-03T13:46:19Z
**Author:** SoWhy
**Comments:** 15
**URL:** https://github.com/immense/Remotely/issues/885

## Description

<!--

Your bug must meet the following requirements, or else it will be closed.  For issues that don't meet these requirements, please reach out to the community on the official subreddit (https://www.reddit.com/r/remotely_app/) or in the Discussion areas on GitHub.

Requirements:
    - You are running Remotely in Docker using our official image
    - 
    - The bug must be related specifically to application code (e.g. not related to hosting, reverse proxy configuration, etc.).
    - It must be immediately reproducible, either in a debug environment or on https://app.remotely.one.  (This doesn't apply to bugs that are clearly code-related.)

    - Repro steps must be included.  The more information, the better.  Pretend you're getting a support request from one of your clients, and think about the kinds of details you want them to include.
-->

**Describe the bug**
Okay, since a few days, even after the upgrade to the 2024 version, I noticed the VM on which Remotely is running will quickly start using a lot of CPU and RAM and I can literally watch the free space being eaten by the "overlay" device:
```
root@remotely-v2:~# df -h
Filesystem                        Size  Used Avail Use% Mounted on
/dev/mapper/VMs-vm--901--disk--0   32G  2.2G   28G   8% /
none                              492K  4.0K  488K   1% /dev
udev                              126G     0  126G   0% /dev/tty
tmpfs                             126G     0  126G   0% /dev/shm
tmpfs                              51G  180K   51G   1% /run
tmpfs                             5.0M     0  5.0M   0% /run/lock
overlay                            32G  2.2G   28G   8% /var/lib/docker/overlay2/cfba3a67d253b20ed27da9ee15744abf16395aa5963cac0866f02033252cc267/merged
root@remotely-v2:~# df -h
Filesystem                        Size  Used Avail Use% Mounted on
/dev/mapper/VMs-vm--901--disk--0   32G  6.1G   24G  21% /
none                              492K  4.0K  488K   1% /dev
udev                              126G     0  126G   0% /dev/tty
tmpfs                             126G     0  126G   0% /dev/shm
tmpfs                              51G  184K   51G   1% /run
tmpfs                             5.0M     0  5.0M   0% /run/lock
overlay                            32G  6.1G   24G  21% /var/lib/docker/overlay2/cfba3a67d253b20ed27da9ee15744abf16395aa5963cac0866f02033252cc267/merged
root@remotely-v2:~# df -h
Filesystem                        Size  Used Avail Use% Mounted on
/dev/mapper/VMs-vm--901--disk--0   32G  7.1G   23G  24% /
none                              492K  4.0K  488K   1% /dev
udev                              126G     0  126G   0% /dev/tty
tmpfs                             126G     0  126G   0% /dev/shm
tmpfs                              51G  184K   51G   1% /run
tmpfs                             5.0M     0  5.0M   0% /run/lock
overlay                            32G  7.1G   23G  24% /var/lib/docker/overlay2/cfba3a67d253b20ed27da9ee15744abf16395aa5963cac0866f02033252cc267/merged
root@remotely-v2:~# df -h
Filesystem                        Size  Used Avail Use% Mounted on
/dev/mapper/VMs-vm--901--disk--0   32G  8.0G   22G  27% /
none                              492K  4.0K  488K   1% /dev
udev                              126G     0  126G   0% /dev/tty
tmpfs                             126G     0  126G   0% /dev/shm
tmpfs                              51G  184K   51G   1% /run
tmpfs                             5.0M     0  5.0M   0% /run/lock
overlay                            32G  8.0G   22G  27% /var/lib/docker/overlay2/cfba3a67d253b20ed27da9ee15744abf16395aa5963cac0866f02033252cc267/merged
root@remotely-v2:~# df -h
Filesystem                        Size  Used Avail Use% Mounted on
/dev/mapper/VMs-vm--901--disk--0   32G  9.0G   21G  31% /
none                              492K  4.0K  488K   1% /dev
udev                              126G     0  126G   0% /dev/tty
tmpfs                             126G     0  126G   0% /dev/shm
tmpfs                              51G  184K   51G   1% /run
tmpfs                             5.0M     0  5.0M   0% /run/lock
overlay                            32G  9.0G   21G  31% /var/lib/docker/overlay2/cfba3a67d253b20ed27da9ee15744abf16395aa5963cac0866f02033252cc267/merged
root@remotely-v2:~# df -h
Filesystem                        Size  Used Avail Use% Mounted on
/dev/mapper/VMs-vm--901--disk--0   32G   10G   20G  34% /
none                              492K  4.0K  488K   1% /dev
udev                              126G     0  126G   0% /dev/tty
tmpfs                             126G     0  126G   0% /dev/shm
tmpfs                              51G  184K   51G   1% /run
tmpfs                             5.0M     0  5.0M   0% /run/lock
overlay                            32G   10G   20G  34% /var/lib/docker/overlay2/cfba3a67d253b20ed27da9ee15744abf16395aa5963cac0866f02033252cc267/merged
[...]
root@remotely-v2:~# df -h
Filesystem                        Size  Used Avail Use% Mounted on
/dev/mapper/VMs-vm--901--disk--0   32G   18G   13G  59% /
none                              492K  4.0K  488K   1% /dev
udev                              126G     0  126G   0% /dev/tty
tmpfs                             126G     0  126G   0% /dev/shm
tmpfs                              51G  184K   51G   1% /run
tmpfs                             5.0M     0  5.0M   0% /run/lock
overlay                            32G   18G   13G  59% /var/lib/docker/overlay2/cfba3a67d253b20ed27da9ee15744abf16395aa5963cac0866f02033252cc267/merged
```
For a while, it will only spike in intervals, filling up a lot of space before returning to 2.2G but after that, it will fill up everything and the CPU will keep being at 99% until I kill the server and restart which will only give me a short reprieve before the circle repeats
htop will show (when it works) that "dotnet Remotely_Server.dll" is the culprit I assume that the problem is some runaway setting for logging but I cannot find it. 

Trying to analyze the mount directory with ncdu shows no additional files, especially not of that size

I set up a new VM with a manual Debian install (instead of the Debian CT), installed Remotely fresh and copied the settings from the old install but the problem persists. It also persists when installing Debian and Docker on a fresh PC and copying over the configuration files from the old install

The problem does not seem to appear before accessing the web interface for the first time. So the service will run with 0% CPU for hours but once you access the web interface, it will start acting up.

Assigning more CPU cores will mitigate the problem but it's not a fix because it will still create spikes in usage, just not as high. 
![2024-05-03 15_45_04-pve - Proxmox Virtual Environment — Mozilla Firefox](https://github.com/immense/Remotely/assets/1128680/954264d8-5b77-436b-b162-192175602e2e)

I found some information online that indicates that it might be related to dotnet 8.x but the problem started with my dotnet 7.x based version before the upgrade. Unfortunately, I cannot remember what changed on that day and I cannot find anything related in the log files. 
![2024-05-03 15_41_52-pve - Proxmox Virtual Environment — Mozilla Firefox](https://github.com/immense/Remotely/assets/1128680/9fc3d947-9b56-43e3-b1f8-f05c081cce6e)

**To Reproduce**
Steps to reproduce the behavior:
1. Install fresh Debian and Remotely
2. Import Remotely.db etc. from old install
3. Access web interface

**Remotely Version**
Server (can be found on about page): 2024.02.23.1927
Agent (can be found in device card): 2024.02.23.1927

**Expected Behavior**
No runaway CPU, memory and I/O usage

**Screenshots**
See https://www.reddit.com/r/remotely_app/comments/1ccyhs8/docker_overlay_eating_up_all_my_space_quickly_and/

**Desktop (please complete the following information):**
 - OS: Windows 10/11 Pro
 - Browser Firefox
 - Version 126

**Additional Context**
Add any other context about the problem here.

- [X] I am running Remotely in Docker and not on my QNAP, Synology, or Internet Connected Toaster

