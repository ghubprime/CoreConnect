// WebAuthn JS interop helpers for CoreConnect passkey registration & login.
window.coreConnectWebAuthn = {
    /**
     * Creates a new credential (registration flow).
     * @param {object} options - CredentialCreateOptions from /api/webauthn/register/options
     * @returns {object} AuthenticatorAttestationRawResponse for POST to /api/webauthn/register
     */
    createCredential: async function (options) {
        // Decode challenge & user.id from Base64URL
        options.challenge = coerceToArrayBuffer(options.challenge);
        options.user.id = coerceToArrayBuffer(options.user.id);

        if (options.excludeCredentials) {
            options.excludeCredentials = options.excludeCredentials.map(c => ({
                ...c,
                id: coerceToArrayBuffer(c.id)
            }));
        }

        const credential = await navigator.credentials.create({ publicKey: options });

        return {
            id: credential.id,
            rawId: coerceToBase64Url(credential.rawId),
            type: credential.type,
            response: {
                attestationObject: coerceToBase64Url(credential.response.attestationObject),
                clientDataJSON: coerceToBase64Url(credential.response.clientDataJSON)
            },
            extensions: credential.getClientExtensionResults()
        };
    },

    /**
     * Retrieves an existing credential (login flow).
     * @param {object} options - AssertionOptions from /api/webauthn/login/options
     * @returns {object} AuthenticatorAssertionRawResponse for POST to /api/webauthn/login
     */
    getCredential: async function (options) {
        options.challenge = coerceToArrayBuffer(options.challenge);

        if (options.allowCredentials) {
            options.allowCredentials = options.allowCredentials.map(c => ({
                ...c,
                id: coerceToArrayBuffer(c.id)
            }));
        }

        const assertion = await navigator.credentials.get({ publicKey: options });

        return {
            id: assertion.id,
            rawId: coerceToBase64Url(assertion.rawId),
            type: assertion.type,
            response: {
                authenticatorData: coerceToBase64Url(assertion.response.authenticatorData),
                clientDataJSON: coerceToBase64Url(assertion.response.clientDataJSON),
                signature: coerceToBase64Url(assertion.response.signature),
                userHandle: assertion.response.userHandle
                    ? coerceToBase64Url(assertion.response.userHandle)
                    : null
            },
            extensions: assertion.getClientExtensionResults()
        };
    }
};

function coerceToArrayBuffer(value) {
    if (typeof value === 'string') {
        // Base64URL → ArrayBuffer
        value = value.replace(/-/g, '+').replace(/_/g, '/');
        const raw = atob(value);
        const buffer = new Uint8Array(raw.length);
        for (let i = 0; i < raw.length; i++) {
            buffer[i] = raw.charCodeAt(i);
        }
        return buffer.buffer;
    }
    return value;
}

function coerceToBase64Url(buffer) {
    const bytes = new Uint8Array(buffer);
    let str = '';
    for (const byte of bytes) {
        str += String.fromCharCode(byte);
    }
    return btoa(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
}
