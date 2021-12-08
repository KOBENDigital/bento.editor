using System.Globalization;

namespace Bento.Core.Services.Interfaces
{
	public interface IPluralizationServiceWrapper
	{
		bool IsPlural(CultureInfo cultureInfo, string word);

		bool IsPlural(string culture, string word);

		bool IsSingular(CultureInfo cultureInfo, string word);

		bool IsSingular(string culture, string word);

		string Pluralize(CultureInfo cultureInfo, string word);

		string Pluralize(string culture, string word);

		string Singularize(CultureInfo cultureInfo, string word);
		
		string Singularize(string culture,string word);
	}
}