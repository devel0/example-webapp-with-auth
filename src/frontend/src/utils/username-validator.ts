import { AuthOptions } from "../../api";
import { ValidatorResult } from "../types/ValidatorResult";
import { nullOrUndefined } from "./utils";

const USERNAME_MIN_LENGTH = 3

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

    for (let i = 0; i < username.length; ++i) {
        if (authOptions.username.allowedUserNameCharacters!.indexOf(username[i]) === -1) {
            isValid = false;
            errors.push(`Character ${username[i]} not allowed`)
        }
    }
    
    return {
        isValid: isValid,
        errors: errors
    } as ValidatorResult
}