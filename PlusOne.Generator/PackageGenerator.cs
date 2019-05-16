namespace PlusOne.Generator
{
	using Core;
	using System;

	public static class PackageGenerator
    {
        private static Random _random = new Random();
        
        public static Package Create()
        {
            return new Package
            {
                Airbill = AirBillGenerator.Create(),
                Origin = AddressGenerator.Create(),
                Destination = AddressGenerator.Create(),
                Service = Service,
                Dimensions = Dimension,
                Signature = Signature,
				Recipient = CreateContact(),
				Shipper = CreateContact()
            };
        }

        public static Package CreatePickup(string postal)
        {
            return new Package
            {
                Airbill = AirBillGenerator.Create(),
                Origin = AddressGenerator.Create(postal),
                Destination = AddressGenerator.Create(),
                Service = Service,
                Recipient = CreateContact(),
                Shipper = CreateContact(),
				Dimensions = Dimension
            };
        }
        
        public static Package CreateDelivery(string postal)
        {
            return new Package
            {
                Airbill = AirBillGenerator.Create(),
                Origin = AddressGenerator.Create(),
                Destination = AddressGenerator.Create(postal),
                Service = Service,
                Recipient = CreateContact(),
                Shipper = CreateContact(),
				Dimensions = Dimension
            };
        }

        public static Contact CreateContact()
        {
	        var firstName = FirstNames.Names[_random.Next(FirstNames.Names.Length)];
	        var lastName = LastNames.Names[_random.Next(LastNames.Names.Length)];

	        return new Contact
	        {
				Name = $"{firstName} {lastName}"
	        };
        }

        private static SignatureRequired Signature
        {
            get
            {
                switch (_random.Next(10))
                {
                    case 0:
                        return SignatureRequired.Adult;
                    case 1:
                    case 2:
                        return SignatureRequired.Consignee;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        return SignatureRequired.Any;
                    default:
                        return SignatureRequired.None;
                }
            }
        }
        
        private static ServiceLevel Service
        {
            get
            {
                switch (_random.Next(10))
                {
                    case 0:
                        return ServiceLevel.NextDayEarly;
                    case 1:
                    case 2:
                    case 3:
                        return ServiceLevel.NextDayMorning;
                    case 4:
                    case 5:
                        return ServiceLevel.NextDayAfternoon;
                    case 6:
                        return ServiceLevel.SecondDay;
                    default:
                        return ServiceLevel.NextDay;
                }
            }
        }

        private static Dimensions Dimension
        {
            get {
                switch (_random.Next(12))
                {
                    case 0: return new Dimensions {Length = 10.0M, Height = 0.25M, Width = 4.0M};
                    case 1: return new Dimensions {Length = 14.0M, Height = 3.25M, Width = 5.0M};
                    case 2: return new Dimensions {Length = 16.0M, Height = 4.00M, Width = 4.0M};
                    case 3: return new Dimensions {Length = 20.0M, Height = 8.5M, Width = 10.0M};
                    case 4: return new Dimensions {Length = 22.0M, Height = 9.0M, Width = 14.0M};
                    case 5: return new Dimensions {Length = 12.0M, Height = 2.50M, Width = 6.0M};
                    case 6: return new Dimensions {Length = 12.0M, Height = 7.25M, Width = 8.0M};
                    case 7: return new Dimensions {Length = 12.0M, Height = 9.00M, Width = 12.0M};
                    case 8: return new Dimensions {Length = 10.0M, Height = 3.25M, Width = 4.0M};
                    case 9: return new Dimensions {Length = 10.0M, Height = 4.00M, Width = 4.75M};
                    case 10: return new Dimensions {Length = 10.0M, Height = 4.25M, Width = 8.0M};
                    default: return new Dimensions {Length = 10.0M, Height = 3.75M, Width = 6.5M};
                }
            }
        }
    }
}