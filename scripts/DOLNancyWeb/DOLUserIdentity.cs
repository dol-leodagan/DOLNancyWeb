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

using Nancy;
using Nancy.Security;

using DOL.Database;

namespace DOLNancyWeb
{
	/// <summary>
	/// DOLUserIdentity Implementation for Nancy Security
	/// Provide access to the "Account" DBObject for this Account
	/// Store GUID for Logout Purpose.
	/// </summary>
	public class DOLUserIdentity : IUserIdentity
	{
		private Account m_dBAccount;
		private Guid m_userGuid;
				
		/// <summary>
		/// Retrieve this UserIdentity DBAccount
		/// </summary>
		public Account DBAccount {
			get { return m_dBAccount; }
		}

		/// <summary>
		/// User Authentication Configured Guid
		/// </summary>
		public Guid UserGuid {
			get { return m_userGuid; }
			set { }
		}
		
		#region IUserIdentity Interface
		/// <summary>
		/// Gets or sets the name of the current user.
		/// </summary>
		public string UserName
		{
			get { return DBAccount.Name; }
			set { }
		}
		
		/// <summary>
		/// Gets or set the claims of the current user.
		/// </summary>
		public IEnumerable<string> Claims
		{
			get { return new List<string>(); }
			set { }
		}
	    #endregion
	    
		public DOLUserIdentity(Account acc, Guid guid)
		{
			m_dBAccount = acc;
			m_userGuid = guid;
		}
	}
}
