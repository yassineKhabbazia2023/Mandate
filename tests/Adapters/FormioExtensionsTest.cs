// <copyright file="FormioExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;

    public class FormioExtensionsTest
    {
        [Fact]
        public void ToCollection()
        {
            var entity = new FormioSubmissionCollection()
            {
                Limit = 1,
                Skip = 2,
                Total = 3,
                Submissions = new List<FormioSubmission>()
                {
                    new FormioSubmission
                    {
                        Id = "id",
                        Owner = string.Empty,
                        Created = "2023-10-10T08:50:03.000Z",
                        Modified = "2023-12-10T08:50:03.000Z",
                        Data = new
                        {
                            accountNumber= "1000326214",
                            companyName= "SPORT FIT SAS",
                            SIRETNumber= "83455379400019",
                            signatoryTitle= "m",
                            signatoryLastName= "BRUNELAT",
                            signatoryFirstName= "OLIVIER",
                            signatoryEmailAddress= "brunelatolivier@gmail.com",
                            headOffice= new
                            {
                                signatoryStreetAddress= "12 RUE DES 2 NATIONS",
                                signatoryAddressComplements= string.Empty,
                                signatoryAddressZipCode= "59250",
                                signatoryAddressCity= "HALLUIN",
                                signatoryAddressCountry= "France",
                            },
                            bankCode= "13507",
                            bankSortCode= "00014",
                            bankAccountNumber= "31464482121",
                            bankCheckNumber= "77",
                            jdcDossierId= "19820673",
                            jdcRibId= "8909441",
                            jdcReleveId= "8909440",
                        },
                    },
                },
            };

            Collection collection = entity.ToCollection();

            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "brunelatolivier@gmail.com");
            var company = new Company(Guid.Empty, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");

            Collection expectedCollection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2023, 10, 10, 8, 50, 3), new DateTime(2023, 12, 10, 8, 50, 3), status);

            collection.Should().BeEquivalentTo(expectedCollection);
        }

        [Fact]
        public void ToCollection_When_NoHeadOffice()
        {
            var entity = new FormioSubmissionCollection()
            {
                Limit = 1,
                Skip = 2,
                Total = 3,
                Submissions = new List<FormioSubmission>()
                {
                    new FormioSubmission
                    {
                        Id = "id",
                        Owner = string.Empty,
                        Created = "2023-10-10T08:50:03.000Z",
                        Modified = "2023-12-10T08:50:03.000Z",
                        Data = new
                        {
                            accountNumber= "1000326214",
                            companyName= "SPORT FIT SAS",
                            SIRETNumber= "83455379400019",
                            signatoryTitle= "m",
                            signatoryLastName= "BRUNELAT",
                            signatoryFirstName= "OLIVIER",
                            signatoryEmailAddress= "brunelatolivier@gmail.com",
                            bankCode= "13507",
                            bankSortCode= "00014",
                            bankAccountNumber= "31464482121",
                            bankCheckNumber= "77",
                            jdcDossierId= "19820673",
                            jdcRibId= "8909441",
                            jdcReleveId= "8909440",
                        },
                    },
                },
            };

            Collection collection = entity.ToCollection();

            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address(null, null, null, null, null);
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "brunelatolivier@gmail.com");
            var company = new Company(Guid.Empty, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");

            Collection expectedCollection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2023, 10, 10, 8, 50, 3), new DateTime(2023, 12, 10, 8, 50, 3), status);

            collection.Should().BeEquivalentTo(expectedCollection);
        }
    }
}
