namespace PlusOne.Core
{
	using System;

	[Flags]
	public enum RouteType { Unknown = 0, Delivery = 1, Pickup = 2, OnCall = 4 }
}