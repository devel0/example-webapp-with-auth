import { ValidatorResult } from "../types/ValidatorResult";

export const emailIsValid = (email: string | null) => {
    if (email === null)
        return {
            isValid: false,
            errors: []
        } as ValidatorResult

    let isValid = true
    const errors: string[] = []

    const testValidEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    if (testValidEmail.test(email) === false) {
        isValid = false;
        errors.push("Invalid email");
    }

    return {
        isValid: isValid,
        errors: errors
    } as ValidatorResult
}