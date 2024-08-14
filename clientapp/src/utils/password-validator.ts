import { ValidatorResult } from "../types/ValidatorResult";

const PASSWORD_MIN_LENGTH = 8

export const passwordIsValid = (password: string) => {
    let isValid = true
    const errors: string[] = []

    if (password.trim().length < PASSWORD_MIN_LENGTH) {
        isValid = false;
        errors.push(`Min length ${PASSWORD_MIN_LENGTH}`)
    }

    const testAtLeastOneLowercase = /^(?=.*[a-z])/;

    if (testAtLeastOneLowercase.test(password) === false) {
        isValid = false;
        errors.push("At least one lowercase letter");
    }

    const testAtLeastOneUppercase = /^(?=.*[A-Z])/;

    if (testAtLeastOneUppercase.test(password) === false) {
        isValid = false;
        errors.push("At least one uppercase letter");
    }

    const testAtLeastOneNumber = /^(?=.*[0-9])/;

    if (testAtLeastOneNumber.test(password) === false) {
        isValid = false;
        errors.push("At least one number");
    }

    const testAtLeastOneSymbol =  /^(?=.*[~`!@#$%^&*()--+={}\[\]|\\:;"'<>,.?/_₹]).*$/;

    if (testAtLeastOneSymbol.test(password) === false) {
        isValid = false;
        errors.push("At least one symbol");
    }

    return {
        isValid: isValid,
        errors: errors
    } as ValidatorResult
}