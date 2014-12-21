/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Linq;

using Nancy;
using Nancy.ViewEngines;
using Nancy.ErrorHandling;

namespace DOLNancyWeb
{
	/// <summary>
	/// Custom Status Handler for Unauthorized, Missing or Internal Server Error.
	/// </summary>
	public class DOLNancyStatusHandler : IStatusCodeHandler
	{
        private readonly IViewFactory _factory;
        private readonly HttpStatusCode[] _supportedStatusCodes = new[] { HttpStatusCode.Forbidden, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError, HttpStatusCode.MethodNotAllowed };

        /// <summary>
        /// Bind to View Factory
        /// </summary>
        /// <param name="factory"></param>
        public DOLNancyStatusHandler(IViewFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Return Custom View on Error Code
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="context"></param>
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            Response response;

            DefaultViewRenderer _viewRenderer = new DefaultViewRenderer(_factory);
            
            var model = new DOLNancyDefaultModel(context);
            model.Title = statusCode.ToString();

            if (statusCode == HttpStatusCode.Forbidden)
            	response = _viewRenderer.RenderView(context, "views/errors/403.sshtml", model);
            else if (statusCode == HttpStatusCode.NotFound)
            	response = _viewRenderer.RenderView(context, "views/errors/404.sshtml", model);
            else if (statusCode == HttpStatusCode.MethodNotAllowed)
                response = _viewRenderer.RenderView(context, "views/errors/405.sshtml", model);
            else
            	response = _viewRenderer.RenderView(context, "views/errors/500.sshtml", model);
            
            // RenderView sets the context.Response.StatusCode to HttpStatusCode.OK
            // so make sure to override it correctly
            response.StatusCode = statusCode;
            context.Response = response;
        }

        /// <summary>
        /// Check if we handle the Status Code.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return this._supportedStatusCodes.Any(s => s == statusCode);
        }
    }
}
