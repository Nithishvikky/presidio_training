import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function userNameValidator():ValidatorFn
{
    return(control:AbstractControl):ValidationErrors|null=>{
        let value = control.value;
        if(value && (value === "admin"|| value === "Admin"|| value === "Root"  || value === "root") )
            return {invalidUsername: true};
        return null;

    }
}
