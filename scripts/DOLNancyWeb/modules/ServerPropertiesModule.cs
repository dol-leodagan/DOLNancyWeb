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
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using Nancy.Session;

using DOL.GS;
using DOL.GS.ServerProperties;
using DOL.Database;

namespace DOLNancyWeb
{
	/// <summary>
	/// Welcome Module For DOLNancyWeb
	/// Display Defaut Home Page with route "/"
	/// </summary>
	public class ServerPropertiesModule : BaseSecuredModule
	{
		public ServerPropertiesModule()
			: base("/serverproperties")
		{
			// Restrict to Admins
			this.RequiresClaims(new string[] { "admin" });
						
			Get["/"] = parameters => {
				// Get Model for Page Display
				var model = new ServerPropertiesModel(this.Context);
				model.Title = "Server Properties";
				model.DataServerProperties = GetAllDomainProperties(this.Session["sp_propertyid"] as string, this.Session["sp_message"] as string);
				this.Session["sp_propertyid"] = null;
				this.Session["sp_message"] = null;
				return View["views/serverproperties.sshtml", model];
			};
			
			Post["/apply/{propertyID}"] = parameters => {
				var propEdit = this.Bind<PropertyEdit>();
				
				var serverProperties = Properties.AllDomainProperties;
				this.Session["sp_propertyid"] = propEdit.PropertyID;
				
				// Edit And Redirect.
				if (serverProperties.ContainsKey(propEdit.PropertyID))
				{
					var serverProperty = serverProperties[propEdit.PropertyID];
					
					object cv = serverProperty.Item2.GetValue(null);
					string currentValue;
					if (cv is double
					    || cv is float
					    || cv is decimal)
					{
						CultureInfo myCIintl = new CultureInfo("en-US", false);
						IFormatProvider provider = myCIintl.NumberFormat;
						currentValue = ((double)cv).ToString(provider);
					}
					else
					{
						currentValue = cv.ToString();
					}
					
					if (propEdit.InitialCurrentValue != currentValue
					   || propEdit.InitialDBValue != serverProperty.Item3.Value)
					{
						this.Session["sp_message"] = "Property has changed in another Session, please Retry Editing !";
					}
					else
					{
						bool update = false;
						if (propEdit.InitialCurrentValue != propEdit.CurrentValue)
						{
							serverProperty.Item2.SetValue(null, Convert.ChangeType(propEdit.CurrentValue, cv.GetType()));
							update = true;
						}
						
						if (propEdit.InitialDBValue != propEdit.DBValue)
						{
							serverProperty.Item3.Value = propEdit.DBValue;
							GameServer.Database.SaveObject(serverProperty.Item3);
							update = true;
						}
						
						if (update)
							this.Session["sp_message"] = "Updated !";
						else
							this.Session["sp_message"] = "Nothing to update !";
					}
						
				}
				else
				{
					this.Session["sp_message"] = "Could not find Property in Runtime !";
				}
				
				
				var response = this.Response.AsRedirect(string.Format("~/serverproperties/#{0}", propEdit.PropertyID));
				response.Headers.Add("Expires", "Sat, 01 Jan 2000 00:00:00 GMT");
				response.Headers.Add("Last-Modified", string.Format("{0:r}", DateTime.UtcNow));
				response.Headers.Add("Cache-Control", "post-check=0, pre-check=0, must-revalidate");
				return response;
			};

			Post["/default/{propertyID}"] = parameters => {
				var propEdit = this.Bind<PropertyEdit>();
				
				var serverProperties = Properties.AllDomainProperties;
				this.Session["sp_propertyid"] = propEdit.PropertyID;
				
				// Edit And Redirect.
				if (serverProperties.ContainsKey(propEdit.PropertyID))
				{
					var serverProperty = serverProperties[propEdit.PropertyID];
					object dv = serverProperty.Item1.DefaultValue;
					string defaultValue;
					if (dv is double
					    || dv is float
					    || dv is decimal)
					{
						CultureInfo myCIintl = new CultureInfo("en-US", false);
						IFormatProvider provider = myCIintl.NumberFormat;
						defaultValue = ((double)dv).ToString(provider);
					}
					else
					{
						defaultValue = dv.ToString();
					}

					serverProperty.Item2.SetValue(null, serverProperty.Item1.DefaultValue);
					serverProperty.Item3.Value = defaultValue;
					GameServer.Database.SaveObject(serverProperty.Item3);

					this.Session["sp_message"] = "All values Reset to Default !";
				}
				else
				{
					this.Session["sp_message"] = "Could not find Property in Runtime !";
				}
				
				var response = this.Response.AsRedirect(string.Format("~/serverproperties/#{0}", propEdit.PropertyID));
				response.Headers.Add("Expires", "Sat, 01 Jan 2000 00:00:00 GMT");
				response.Headers.Add("Last-Modified", string.Format("{0:r}", DateTime.UtcNow));
				response.Headers.Add("Cache-Control", "post-check=0, pre-check=0, must-revalidate");
				return response;
			};
		}
		
		class PropertyEdit
		{
			public string PropertyID { get; set; }
			public string InitialDBValue { get; set; }
			public string InitialCurrentValue { get; set; }
			public string DBValue { get; set; }
			public string CurrentValue { get; set; }
		}
		
		public class PropertyDisplay
		{
			public string Description { get; set; }
			public string Category { get; set; }
			public string DefaultValue { get; set; }
			public string CurrentValue { get; set; }
			public string DataValue { get; set; }
			public bool IsTextArea { get; set; }
			public string Regex { get; set; }
			public bool InDB { get; set; }
			public bool IsDefault { get; set; }
			public string Message { get; set; }
		}
		
		public static IDictionary<string, PropertyDisplay> GetAllDomainProperties(string key = null, string message = null)
		{
			return Properties.AllDomainProperties
				.ToDictionary(k => k.Key, v => {
				              	string dv;
                               	string cv;
                               	string regex;
                               	if (v.Value.Item1.DefaultValue is float
                               	    || v.Value.Item1.DefaultValue is double
                               	    || v.Value.Item1.DefaultValue is decimal)
                               	{
									CultureInfo myCIintl = new CultureInfo("en-US", false);
									IFormatProvider provider = myCIintl.NumberFormat;
									dv = ((double)v.Value.Item1.DefaultValue).ToString(provider);
									cv = ((double)v.Value.Item2.GetValue(null)).ToString(provider);
									regex = @"[-+]?[0-9]*\\.?[0-9]+";
                               	}
                               	else
                               	{
                               		if (v.Value.Item1.DefaultValue is sbyte
                               		   || v.Value.Item1.DefaultValue is short
                               		   || v.Value.Item1.DefaultValue is int
                               		   || v.Value.Item1.DefaultValue is long)
                               		{
                               			regex = @"[-+]?[0-9]+";
                               		}
                               		else if (v.Value.Item1.DefaultValue is byte
                               		         || v.Value.Item1.DefaultValue is ushort
                               		         || v.Value.Item1.DefaultValue is uint
                               		         || v.Value.Item1.DefaultValue is ulong)
                               		{
                               			regex = "[0-9]+";
                               		}
                               		else if (v.Value.Item1.DefaultValue is bool)
                               		{
                               			regex = @"[Ff][Aa][Ll][Ss][Ee]|[Tt][Rr][Uu][Ee]";
                               		}
                               		else
                               		{
                               			regex = ".*";
                               		}
                               		
                               		dv = v.Value.Item1.DefaultValue.ToString();
                               		cv = v.Value.Item2.GetValue(null).ToString();
                               	}
                               	
                               	return new PropertyDisplay
                               	{
                               		Description = v.Value.Item1.Description,
                               		Category = v.Value.Item1.Category,
                               		DefaultValue = dv,
                               		CurrentValue = cv,
                               		DataValue = v.Value.Item3.Value,
                               		IsTextArea = v.Value.Item1.DefaultValue is string,
                               		Regex = regex,
                               		InDB = v.Value.Item3.Value.Equals(cv),
                               		IsDefault = v.Value.Item3.Value.Equals(dv) && cv.Equals(dv),
                               		Message = v.Key.Equals(key) ? message : (string)null,
                               	};
				              });
		}
	}
}
