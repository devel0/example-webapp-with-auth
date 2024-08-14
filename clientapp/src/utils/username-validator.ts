import { ValidatorResult } from "../types/ValidatorResult";

const USERNAME_MIN_LENGTH = 4

export const usernameIsValid = (username: string) => {
    let isValid = true
    const errors: string[] = []

    if (username.trim().length < USERNAME_MIN_LENGTH) {
        isValid = false;
        errors.push(`Min length ${USERNAME_MIN_LENGTH}`)
    }    

    if (username.indexOf(' ') !== -1) {
        isValid = false;
        errors.push("Contains no whitespace");
    }     

    const testAllowedCharacters = /^[a-zA-Z0-9\.\-_]*$/;

    if (testAllowedCharacters.test(username) === false) {
        isValid = false;
        errors.push("Allowed chars a-z A-Z 0-9 . _ -");
    }

    return {
        isValid: isValid,
        errors: errors
    } as ValidatorResult
}