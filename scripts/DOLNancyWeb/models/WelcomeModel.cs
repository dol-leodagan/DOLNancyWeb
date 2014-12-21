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

using Nancy;

using DOL;

namespace DOLNancyWeb
{
	/// <summary>
	/// Welcome Page Model with Server Status
	/// </summary>
	public class WelcomeModel : DOLNancyDefaultModel
	{
		/// <summary>
		/// Network Bytes Count In
		/// </summary>
		public double BytesIn
		{
			get { return Math.Round(((double)Statistics.BytesIn)/1024/1024, 2); }
		}

		/// <summary>
		/// Network Bytes Count Out
		/// </summary>
		public double BytesOut
		{
			get { return Math.Round(((double)Statistics.BytesOut)/1024/1024, 2); }
		}
		
		/// <summary>
		/// Memory Usage in MB
		/// </summary>
		public double Memory
		{
			get { return Math.Round(((double)GC.GetTotalMemory(false))/1024/1024, 1); }
		}
		
		public WelcomeModel(NancyContext context)
			: base(context)
		{
			
		}
	}
}