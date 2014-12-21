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
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using Nancy.Extensions;

using DOL.GS;

namespace DOLNancyWeb
{
	/// <summary>
	/// This module Handle authentication forms
	/// </summary>
	public class AuthenticationModule : BasePublicModule
	{
		/// <summary>
		/// Anti Brute Force expiry
		/// </summary>
		private const int ANTI_BRUTEFORCE_TIMER_CLEANUP = 60000;

		/// <summary>
		/// Anti Brute Force Time between attempt
		/// </summary>
		private const int ANTI_BRUTEFORCE_TIME_LIMIT = 3000;

		/// <summary>
		/// Timer for regular Attempts Cleanup 
		/// </summary>
		private static readonly System.Threading.Timer m_antibruteForce = new System.Threading.Timer(BruteForceCallback, null, ANTI_BRUTEFORCE_TIMER_CLEANUP, System.Threading.Timeout.Infinite);
		
		/// <summary>
		/// Static Dictionary for registering Attempt by Client IP.
		/// </summary>
		private static readonly ReaderWriterDictionary<string, long> m_loginAttempt = new ReaderWriterDictionary<string, long>();
		
		public AuthenticationModule()
			: base()
		{			
			// Login Form
			Get["/login"] = parameters => View["views/login.sshtml", new AuthenticationModel(this.Context)];

			// Logout URI
			Get["/logout"] = parameters => {				
				// check is user is logged.
				if (this.Context != null && this.Context.CurrentUser != null && this.Context.CurrentUser is DOLUserIdentity)
					DOLUserMapper.LogoutUser(((DOLUserIdentity)this.Context.CurrentUser).UserGuid);
				
				return this.Logout("~/");
			};
			
			// Login Form Posting
			Post["/login"] = parameters => {
				// Param From Login Form
				var loginParams = this.Bind<LoginParams>();
				
				string errorMessage;
				Guid guid;
				bool ok = DOLUserMapper.ValidateUser(loginParams.Username, loginParams.Password, out guid, out errorMessage);
				
				// Test Anti Brute Force
				long lastAttempt;
				if (m_loginAttempt.TryGetValue(this.Context.Request.UserHostAddress, out lastAttempt))
				{
					if ((GameTimer.GetTickCount() - lastAttempt) < ANTI_BRUTEFORCE_TIME_LIMIT)
					{
						var model = new AuthenticationModel(this.Context);
						model.Message = "Please wait some time before trying to Log In again...";
						return View["views/login.sshtml",model];
					}
				}
				
				// Wrong login display form with error message
				if (!ok)
				{
					// Register anti brute force
					m_loginAttempt[this.Context.Request.UserHostAddress] = GameTimer.GetTickCount();
				
					var model = new AuthenticationModel(this.Context);
					model.Message = string.Format("Error While Authenticating User {0} - {1}", loginParams.Username, errorMessage);
					return View["views/login.sshtml",model];
				}
				
				// Login successful redirect and continue !
				return this.Login(guid);
			};
		}
		
		/// <summary>
		/// Anti Brute Force Timer Callback
		/// </summary>
		/// <param name="state"></param>
		private static void BruteForceCallback(object state)
		{
			foreach (var ip in m_loginAttempt.Where(kvp => (GameTimer.GetTickCount() - kvp.Value) > ANTI_BRUTEFORCE_TIMER_CLEANUP).Select(kvp => kvp.Key).ToArray())
			{
				long dummy;
				m_loginAttempt.TryRemove(ip, out dummy);
			}
			
			//Loop the Timer
			m_antibruteForce.Change(ANTI_BRUTEFORCE_TIMER_CLEANUP, System.Threading.Timeout.Infinite);
		}
		
		/// <summary>
		/// Login Params Model
		/// </summary>
		public class LoginParams
		{
			public string Username { get; set; }
			public string Password { get; set; }
		}

	}
}
