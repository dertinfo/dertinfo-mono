/**
 * Returns the user's token. This function simply needs to make a retrieval
 * call to wherever the token is stored. In many cases, the token will be
 * stored in local storage or session storage.
 * @returns Returns the user's token.
 */
export function tokenGetter(): string {
    return localStorage.getItem('access_token');
}

/**
 * Factory for generating the configuration for the JwtModule.
 * @returns an object containing the desired options under the 'options' property
 * and a method to update the allowedDomains.
 */
export function jwtConfigurationFactory(): {
    options: () =>
        {
            tokenGetter: () => string;
            allowedDomains: any[];
        };
    updateAllowedDomains: (newAllowedDomains: string[]) => void
} {
    const allowedDomains: string[] = [];

    /**
     * Clears the old entries and sets the allowed domains to the given value
     */
    function updateAllowedDomains(newAllowedDomains: string[]): void {
    
        // Insert into allowed domains the passed domains
        allowedDomains.push(...newAllowedDomains);
        // note - we should really prevent duplicates here.
    }

    return {
        updateAllowedDomains,
        options: () => ({
            tokenGetter,
            allowedDomains,
        })
    };
}

/**
 * Saves an instance of the object returned by jwtConfigurationFactory.
 * the options property can be passed to angular-jwt.
 * This object can be used to access the allowedDomains afterwards.
 */
export const jwtOptions = jwtConfigurationFactory();