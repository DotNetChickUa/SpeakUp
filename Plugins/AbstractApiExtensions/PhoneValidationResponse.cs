namespace AbstractApiExtensions;

public class PhoneValidationResponse
{
    public string phone_number { get; set; }
    public Phone_Format phone_format { get; set; }
    public Phone_Carrier phone_carrier { get; set; }
    public Phone_Location phone_location { get; set; }
    public Phone_Messaging phone_messaging { get; set; }
    public Phone_Validation phone_validation { get; set; }
    public Phone_Registration phone_registration { get; set; }
    public Phone_Risk phone_risk { get; set; }
    public Phone_Breaches phone_breaches { get; set; }
}

public class Phone_Format
{
    public string international { get; set; }
    public string national { get; set; }
}

public class Phone_Carrier
{
    public string name { get; set; }
    public string line_type { get; set; }
    public string mcc { get; set; }
    public string mnc { get; set; }
}

public class Phone_Location
{
    public string country_name { get; set; }
    public string country_code { get; set; }
    public string country_prefix { get; set; }
    public string region { get; set; }
    public string city { get; set; }
    public string timezone { get; set; }
}

public class Phone_Messaging
{
    public string sms_domain { get; set; }
    public string sms_email { get; set; }
}

public class Phone_Validation
{
    public bool is_valid { get; set; }
    public string line_status { get; set; }
    public bool is_voip { get; set; }
    public object minimum_age { get; set; }
}

public class Phone_Registration
{
    public object name { get; set; }
    public object type { get; set; }
}

public class Phone_Risk
{
    public string risk_level { get; set; }
    public bool is_disposable { get; set; }
    public bool is_abuse_detected { get; set; }
}

public class Phone_Breaches
{
    public object total_breaches { get; set; }
    public object date_first_breached { get; set; }
    public object date_last_breached { get; set; }
    public object[] breached_domains { get; set; }
}
