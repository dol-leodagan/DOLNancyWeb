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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;

using DOL.GS;
using DOL.Database;
using DOL.GS.PacketHandler.Client.v168;

using log4net;

namespace DOLNancyWeb
{
	/// <summary>
	/// User Mapper For Dol Form Authentication Users.
	/// Uses Static Dictionary to match Dynamic GUID to Accounts.
	/// </summary>
	public class DOLUserMapper : IUserMapper
	{
		/// <summary>
		/// Memory Static Mapping Dictionary (with long for init time storing)
		/// </summary>
		private static readonly ReaderWriterDictionary<Guid, IUserIdentity> m_userIdentityMapper = new ReaderWriterDictionary<Guid, IUserIdentity>();
		
		/// <summary>
		/// Register an Authenticated User.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="identity"></param>
		private static void AddAuthenticatedUser(Guid identifier, IUserIdentity identity)
		{
			m_userIdentityMapper[identifier] = identity;
		}
		
		/// <summary>
		/// Remove a Registered User based on GUID.
		/// </summary>
		/// <param name="identifier"></param>
		private static void RemoveAuthenticatedUser(Guid identifier)
		{
			IUserIdentity dummy;
			m_userIdentityMapper.TryRemove(identifier, out dummy);
		}
		
		/// <summary>
		/// Remove a Registered User based on his identity (Unsafe)
		/// </summary>
		/// <param name="identity"></param>
		private static void RemoveAuthenticatedUser(IUserIdentity identity)
		{
			try
			{
				m_userIdentityMapper.FreezeWhile(dict => dict.Remove(dict.FirstOrDefault(e => e.Value == identity).Key));
			}
			catch
			{
			}
		}
		
		/// <summary>
		/// Validate a User against database and init its GUID.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="errormessage"></param>
		/// <returns></returns>
		public static bool ValidateUser(string username, string password, out Guid guid, out string errormessage)
		{
			guid = Guid.NewGuid();

			// Validate Fields
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				errormessage = "Empty Username or Password...";
				return false;
			}
			
			
			// Try to find the player
			Account playerAccount = null;
			try
			{
				playerAccount = GameServer.Database.FindObjectByKey<Account>(GameServer.Database.Escape(username));
			}
			catch
			{
				errormessage = "Error while querying Database !";
				return false;
			}
			
			// Validate account
			if (playerAccount != null && playerAccount.Password.Equals(LoginRequestHandler.CryptPassword(password)))
			{
				// Success
				DOLUserMapper.AddAuthenticatedUser(guid, new DOLUserIdentity(playerAccount, guid));
				errormessage = string.Empty;
				return true;
			}
			else
			{
				errormessage = "Wrong Login or Password !";
			}
			
			return false;
		}
		
		/// <summary>
		/// Log out a previously Validated User
		/// </summary>
		public static void LogoutUser(Guid identifier)
		{
			RemoveAuthenticatedUser(identifier);
		}
		
		/// <summary>
		/// Map Received GUID from Session to a Real User Object.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
		{
			IUserIdentity user;
			
			if (m_userIdentityMapper.TryGetValue(identifier, out user))
			{
				return user;
			}
			
			return null;
		}
	}
}
