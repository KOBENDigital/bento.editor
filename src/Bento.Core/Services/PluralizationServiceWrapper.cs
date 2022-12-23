using System.Globalization;
using Bento.Core.Services.Interfaces;
using Pluralize.NET;

namespace Bento.Core.Services
{
	public class PluralizationServiceWrapper : IPluralizationServiceWrapper
	{
		private readonly IPluralize _pluralize;

		public PluralizationServiceWrapper(IPluralize pluralize)
		{
			_pluralize = pluralize;
		}

		public bool IsPlural(CultureInfo cultureInfo, string word)
		{
			return _pluralize.IsPlural(word);
		}

		public bool IsPlural(string culture, string word)
		{
			return _pluralize.IsPlural(word);
		}

		public bool IsSingular(CultureInfo cultureInfo, string word)
		{
			return _pluralize.IsSingular(word);
		}

		public bool IsSingular(string culture, string word)
		{
			return _pluralize.IsSingular(word);
		}

		public string Pluralize(CultureInfo cultureInfo, string word)
		{
			return _pluralize.Pluralize(word);
		}

		public string Pluralize(string culture, string word)
		{
			return _pluralize.Pluralize(word);
		}

		public string Singularize(CultureInfo cultureInfo, string word)
		{
			return _pluralize.Singularize(word);
		}

		public string Singularize(string culture, string word)
		{
			return _pluralize.Singularize(word);
		}
	}
}