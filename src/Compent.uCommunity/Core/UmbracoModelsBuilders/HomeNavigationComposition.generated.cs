//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.5.96
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedContentModels
{
	// Mixin content Type 1067 with alias "homeNavigationComposition"
	/// <summary>Home Navigation Composition</summary>
	public partial interface IHomeNavigationComposition : IPublishedContent
	{
		/// <summary>Is show in Home Navigation</summary>
		bool IsShowInHomeNavigation { get; }
	}

	/// <summary>Home Navigation Composition</summary>
	[PublishedContentModel("homeNavigationComposition")]
	public partial class HomeNavigationComposition : PublishedContentModel, IHomeNavigationComposition
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "homeNavigationComposition";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public HomeNavigationComposition(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<HomeNavigationComposition, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Is show in Home Navigation
		///</summary>
		[ImplementPropertyType("isShowInHomeNavigation")]
		public bool IsShowInHomeNavigation
		{
			get { return GetIsShowInHomeNavigation(this); }
		}

		/// <summary>Static getter for Is show in Home Navigation</summary>
		public static bool GetIsShowInHomeNavigation(IHomeNavigationComposition that) { return that.GetPropertyValue<bool>("isShowInHomeNavigation"); }
	}
}