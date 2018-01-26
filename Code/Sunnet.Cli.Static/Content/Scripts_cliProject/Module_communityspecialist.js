



function InitializeAccess(role) {
    switch (role)
    {
        case "Super_admin":
        case "Statisticians":
        case "Intervention_manager":
        case "Intervention_support_personnel":
        case "Administrative_personnel":
            //max
            break;
        case "Content_personnel":
        case "Statewide":
        case "Auditor":
            //only read
            break;
        case "Coordinator":
        case "Mentor_coach":
        case "Video_coding_analyst":
        case "Principal":
        case "Principal_Delegate":
        case "Teacher":
        case "Parent":
            // cannot view
            break;
        case "TRS_Specialist":
        case "TRS_Specialist_Delegate":
        case "School_Specialist":
        case "School_Specialist_Delegate":
        case "District_Community_Specialist":
        case "Community_Specialist_Delegate":
            //otherfiled is rw ,but notes cannot be viewed
            $("#divCommunityNotes").hide();
            break;
        case "Community":
        case "District_Community_Delegate":

            //birthdate/gender/ethnicity/primarylanguage/sencondarylanguage
            //totalyearsas specialist/In what areas have you received PD/heighest education level/
            //certificate and credential/	r
            //note x
            //other rw
            $("#BirthDate").add("#Gender1").add("#Gender2").add("#PrimaryLanguageId")
                .add("#TotalYearCurrentRole").add("#EducationLevel")
                .each(function () {
                    $(this).prop("disabled", "true");
                });
            $("#divCommunityNotes").hide();
            break;
    }
}







