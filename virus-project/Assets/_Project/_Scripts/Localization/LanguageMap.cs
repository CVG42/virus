public static class LanguageMap
{
    public static string ToDB(string unityLang)
    {
        return unityLang switch
        {
            "English" => "en",
            "Spanish" => "es",
            "Portuguese" => "pt",
            "Japanese" => "jp",
            _ => "en"
        };
    }

    public static string FromDB(string code)
    {
        return code switch
        {
            "en" => "English",
            "es" => "Spanish",
            "pt" => "Portuguese",
            "jp" => "Japanese",
            _ => "English"
        };
    }
}
