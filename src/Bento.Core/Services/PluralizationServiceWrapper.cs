using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using Bento.Core.Services.Interfaces;

namespace Bento.Core.Services
{
	public class PluralizationServiceWrapper : IPluralizationServiceWrapper
	{
		public bool IsPlural(CultureInfo cultureInfo, string word)
		{
			return CreateService(cultureInfo).IsPlural(word);
		}

		public bool IsPlural(string culture, string word)
		{
			return CreateService(culture).IsPlural(word);
		}

		public bool IsSingular(CultureInfo cultureInfo, string word)
		{
			return CreateService(cultureInfo).IsSingular(word);
		}

		public bool IsSingular(string culture, string word)
		{
			return CreateService(culture).IsSingular(word);
		}

		public string Pluralize(CultureInfo cultureInfo, string word)
		{
			return CreateService(cultureInfo).Pluralize(word);
		}

		public string Pluralize(string culture, string word)
		{
			return CreateService(culture).Pluralize(word);
		}

		public string Singularize(CultureInfo cultureInfo, string word)
		{
			return CreateService(cultureInfo).Singularize(word);
		}

		public string Singularize(string culture, string word)
		{
			return CreateService(culture).Singularize(word);
		}

		private static PluralizationService CreateService(CultureInfo cultureInfo)
		{
			return PluralizationService.CreateService(cultureInfo);
		}

		private static PluralizationService CreateService(string culture)
		{
			return PluralizationService.CreateService(new CultureInfo(culture));
		}
	}
}