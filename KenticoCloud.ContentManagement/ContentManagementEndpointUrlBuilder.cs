﻿using System;
using System.Collections.Generic;
using System.Web;
using KenticoCloud.ContentManagement.Models.Assets;

namespace KenticoCloud.ContentManagement
{
    internal sealed class EndpointUrlBuilder
    {
        private const int URI_MAX_LENGTH = 65519;

        private const string URL_TEMPLATE_PROJECT = "projects/{0}";

        private const string URL_ITEM = "/items";
        private const string URL_TEMPLATE_ITEM_ID = "/items/{0}";
        private const string URL_TEMPLATE_ITEM_EXTERNAL_ID = "/items/external-id/{0}";
        private const string URL_TEMPLATE_ITEM_CODENAME = "/items/codename/{0}";

        private const string URL_VARIANT = "/variants";
        private const string URL_TEMPLATE_VARIANT_ID = "/variants/{0}";
        private const string URL_TEMPLATE_VARIANT_CODENAME = "/variants/codename/{0}";

        private readonly ContentManagementOptions _options;

        internal EndpointUrlBuilder(ContentManagementOptions options)
        {
            _options = options;
        }

        #region Variants

        internal string BuildListVariantsUrl(ContentItemIdentifier identifier)
        {
            var itemSegment = GetItemUrlSegment(identifier);

            return GetUrl(string.Concat(itemSegment, URL_VARIANT));
        }

        internal string BuildVariantsUrl(ContentItemVariantIdentifier identifier)
        {
            var itemSegment = GetItemUrlSegment(identifier);
            var variantSegment = GetVariantUrlSegment(identifier);

            return GetUrl(string.Concat(itemSegment, variantSegment));
        }

        #endregion

        #region Items

        internal string BuildItemsUrl()
        {
            return GetUrl(URL_ITEM);
        }

        internal string BuildItemUrl(ContentItemIdentifier identifier)
        {
            var itemSegment = GetItemUrlSegment(identifier);
            return GetUrl(itemSegment);
        }

        #endregion

        private string GetUrl(string path, params string[] parameters)
        {
            var projectSegment = string.Format(URL_TEMPLATE_PROJECT, _options.ProjectId);

            var endpointUrl = string.Format(_options.Endpoint, projectSegment);
            var url = string.Concat(endpointUrl, path);

            if (parameters != null && parameters.Length > 0)
            {
                var joinedQuery = string.Join("&", parameters);
                url = string.Concat(url, "?", HttpUtility.ParseQueryString(joinedQuery));
            }

            if (url.Length > URI_MAX_LENGTH)
            {
                throw new UriFormatException("The request url is too long. Split your query into multiple calls.");
            }

            return url;
        }

        private string GetItemUrlSegment(ContentItemVariantIdentifier identifier)
        {

            if (!string.IsNullOrEmpty(identifier.ItemId))
            {
                return string.Format(URL_TEMPLATE_ITEM_ID, identifier.ItemId);
            }

            if (!string.IsNullOrEmpty(identifier.ItemCodename))
            {
                return string.Format(URL_TEMPLATE_ITEM_CODENAME, identifier.ItemCodename);
            }

            if (!string.IsNullOrEmpty(identifier.ItemExternalId))
            {
                return string.Format(URL_TEMPLATE_ITEM_EXTERNAL_ID, identifier.ItemExternalId);
            }

            throw new ArgumentException("You must provide item's id, codename or externalId");
        }

        private string GetItemUrlSegment(ContentItemIdentifier identifier)
        {

            if (!string.IsNullOrEmpty(identifier.ItemId))
            {
                return string.Format(URL_TEMPLATE_ITEM_ID, identifier.ItemId);
            }

            if (!string.IsNullOrEmpty(identifier.ItemCodename))
            {
                return string.Format(URL_TEMPLATE_ITEM_CODENAME, identifier.ItemCodename);
            }

            if (!string.IsNullOrEmpty(identifier.ItemExternalId))
            {
                return string.Format(URL_TEMPLATE_ITEM_EXTERNAL_ID, identifier.ItemExternalId);
            }

            throw new ArgumentException("You must provide item's id, codename or externalId");
        }

        private string GetVariantUrlSegment(ContentItemVariantIdentifier identifier)
        {

            if (!string.IsNullOrEmpty(identifier.LanguageId))
            {
                return string.Format(URL_TEMPLATE_VARIANT_ID, identifier.LanguageId);
            }

            if (!string.IsNullOrEmpty(identifier.LanguageCodename))
            {
                return string.Format(URL_TEMPLATE_VARIANT_CODENAME, identifier.LanguageCodename);
            }

            return URL_VARIANT;
        }

        public string BuildUploadAssetUrl(string fileName)
        {
            throw new NotImplementedException();
        }

        public string BuildAssetListingUrl(string continuationToken = null)
        {
            return continuationToken != null ? GetUrl($"/assets" ,$"continuationToken={continuationToken}") : GetUrl("/assets");
        }

        public string BuildAssetsUrlFromId(string id)
        {
            return GetUrl($"/assets/{id}");
        }

        public string BuildAssetsUrlFromExternalId(string externalId)
        {
            return GetUrl($"/assets/external-id/{externalId}");
        }
    }
}