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

using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;

namespace DOLNancyWeb
{
	/// <summary>
	/// User Mapper For Dol Form Authentication Users.
	/// </summary>
	public class DOLUserMapper : IUserMapper
	{
		private static Dictionary<Guid, IUserIdentity> m_userIdentityMapper = new Dictionary<Guid, IUserIdentity>();
		
		/// <summary>
		/// Register an Authenticated User.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="identity"></param>
		public static void AddAuthenticatedUser(Guid identifier, IUserIdentity identity)
		{
			m_userIdentityMapper[identifier] = identity;
		}
		
		/// <summary>
		/// Remove a Registered User based on GUID.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="identity"></param>
		public static void RemoveAuthenticatedUser(Guid identifier)
		{
			if (m_userIdentityMapper.ContainsKey(identifier))
				m_userIdentityMapper.Remove(identifier);
		}
		
		/// <summary>
		/// Remove a Registered User based on his identity (Unsafe)
		/// </summary>
		/// <param name="identity"></param>
		public static void RemoveAuthenticatedUser(IUserIdentity identity)
		{
			try
			{
				m_userIdentityMapper.Remove(m_userIdentityMapper.FirstOrDefault(e => e.Value == identity).Key);
			}
			catch
			{
			}
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
				return user;
			
			return null;
		}
	}
}
