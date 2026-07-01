namespace DertInfo.Models.System.Enumerations
{
    public enum EventSettingType
    {
        EMAIL_FROM_ADDRESS, // redundant 
        EMAIL_FROM_NAME, // redundant 
        DEFAULT_ATTENDANCECLASSIFICATION_ID, // redundant 
        RESULTS_PUBLISHED, 
        SMTP_EMAIL_SERVER, // redundant 
        SMTP_EMAIL_USERNAME, // redundant 
        SMTP_EMAIL_PASSWORD, // redundant 
        EMAIL_BCC1,
        EMAIL_BCC2,
        EMAIL_BCC3,
        EMAIL_ATTACHMENT_LOCATION, // redundant 
        REGISTRATION_OPEN_DATE, // redundant 
        REGISTRATION_CLOSE_DATE, // redundant 
        INVOICE_PAYMENTCLOSE_DATE, 
        TEAM_ENTRY_FEE, // redundant 
        MAINCOMP_COMPETITION_ID, // compeition
        MAINCOMP_CHARACTERS_SCORECATEGORY_ID, // competition
        MAINCOMP_BUZZ_SCORECATEGORY_ID, // competition
        MAINCOMP_MUSIC_SCORECATEGORY_ID // competition
    };
}