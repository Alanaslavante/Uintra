﻿using System;
using System.Web;
using uIntra.Core.Extentions;

namespace uIntra.Core
{
    public class CookieProvider : ICookieProvider
    {
        private readonly HttpContext _httpContext;

        public CookieProvider(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public HttpCookie Get(string name)
        {
            return _httpContext.Request.Cookies[name];
        }

        public void Save(HttpCookie cookie)
        {
            cookie.Domain = GetDomain();
            _httpContext.Response.Cookies.Add(cookie);
        }

        public void Save(string name, string value, DateTime expireDate)
        {
            var cookie = new HttpCookie(name)
            {
                Name = name,
                Value = value,
                Expires = expireDate
            };

            Save(cookie);
        }

        public bool Exists(string name)
        {
            var cookie = Get(name);
            return cookie != null && cookie.Value.IsNotNullOrEmpty();
        }

        public void Delete(string name)
        {
            var cookie = Get(name);
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
            }
        }

        protected virtual string GetDomain()
        {
            return _httpContext.Request.Url.Host;
        }
    }
}
