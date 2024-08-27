import { AuthOptions } from "../../api";
import { ValidatorResult } from "../types/ValidatorResult";

export const passwordIsValid = (authOptions: AuthOptions, password: string) => {
    let isValid = true
    const errors: string[] = []

    if (password.trim().length < authOptions.password.requiredLength) {
        isValid = false;
        errors.push(`Min length ${authOptions.password.requiredLength}`)
    }

    if (authOptions.password.requireLowercase) {
        const testAtLeastOneLowercase = /^(?=.*[a-z])/;

        if (testAtLeastOneLowercase.test(password) === false) {
            isValid = false;
            errors.push("At least one lowercase letter");
        }
    }

    if (authOptions.password.requireUppercase) {
        const testAtLeastOneUppercase = /^(?=.*[A-Z])/;

        if (testAtLeastOneUppercase.test(password) === false) {
            isValid = false;
            errors.push("At least one uppercase letter");
        }
    }

    if (authOptions.password.requireDigit) {
        const testAtLeastOneNumber = /^(?=.*[0-9])/;

        if (testAtLeastOneNumber.test(password) === false) {
            isValid = false;
            errors.push("At least one number");
        }
    }

    if (authOptions.password.requireNonAlphanumeric) {
        const testAtLeastOneSymbol = /^(?=.*[~`!@#$%^&*()--+={}\[\]|\\:;"'<>,.?/_₹]).*$/;

        if (testAtLeastOneSymbol.test(password) === false) {
            isValid = false;
            errors.push("At least one symbol");
        }
    }

    return {
        isValid: isValid,
        errors: errors
    } as ValidatorResult
}