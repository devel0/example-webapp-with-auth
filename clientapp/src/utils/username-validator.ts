import { AuthOptions } from "../../api";
import { ValidatorResult } from "../types/ValidatorResult";
import { nullOrUndefined } from "./utils";

const USERNAME_MIN_LENGTH = 4

export const usernameIsValid = (authOptions: AuthOptions, username: string | null | undefined) => {
    if (username === null || username === undefined) return {
        isValid: false,
        errors: []
    } as ValidatorResult

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