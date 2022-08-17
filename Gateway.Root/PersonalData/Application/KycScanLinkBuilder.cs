namespace Gateway.Root.PersonalData.Application;

public class KycScanLinkBuilder
{
	private readonly string _domain;
	private readonly LinkGenerator _linkGenerator;

	public KycScanLinkBuilder(string domain, LinkGenerator linkGenerator)
	{
		_domain = domain;
		_linkGenerator = linkGenerator;
	}

	public string BuildLinkToKycScan(Guid kycScanId)
	{
		var actionUri = _linkGenerator.GetPathByAction("Get", "KycScan", new
		{
			kycScanId
		});

		return $"{_domain}{actionUri}";
	}
}