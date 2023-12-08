// <copyright file="FormIoAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;
    using Newtonsoft.Json;

    public class FormIoAdapterTest
    {
        [Fact]
        public async Task GetSubmissionMandateAsync()
        {
            Bban bban = new Bban("13507", "00014", "31464482121", "77", "8909440", null);
            string data =
            @"[{
                  id: ""5d9ef125d5a3477ca6026100"",
                  data: {
                    accountNumber: ""1000326214"",
                    companyName: ""SPORT FIT SAS"",
                    SIRETNumber: ""83455379400019"",
                    signatoryTitle: ""m"",
                    signatoryLastName: ""BRUNELAT"",
                    signatoryFirstName: ""OLIVIER"",
                    signatoryEmailAddress: ""brunelatolivier@gmail.com"",
                    headOffice: {
                        signatoryStreetAddress: ""12 RUE DES 2 NATIONS"",
                        signatoryAddressComplements: """",
                        signatoryAddressZipCode: ""59250"",
                        signatoryAddressCity: ""HALLUIN"",
                        signatoryAddressCountry: ""France""
                    },
                    bankCode: ""13507"",
                    bankSortCode: ""00014"",
                    bankAccountNumber: ""31464482121"",
                    bankCheckNumber: ""77"",
                    companyId: """",
                    userId: ""0896c1f7-4521-4c57-968f-d74e620b7cbc"",
                    jdcDossierId: ""19820673"",
                    jdcRibId: ""8909441"",
                    jdcReleveId: ""8909440""
                },
                created: ""2019-10-10T08:54:03.000Z"",
                modified: ""2019-10-10T08:54:03.000Z""
            }]";

            var sub = new FormioSubmissionCollection()
            {
                Limit = 1,
                Skip = 0,
                Total = 1,
                Submissions = JsonConvert.DeserializeObject<List<FormioSubmission>>(data)!,
            };

            var formIoClient = new Mock<IFormIoClient>(MockBehavior.Strict);
            formIoClient.Setup(item => item.GetSubmissionMandateAsync("13507", "00014", "31464482121", "77", It.IsAny<FormioAuthToken>()))
                .ReturnsAsync(sub)
                .Verifiable();

            var adapter = new FormIoAdapter(formIoClient.Object);

            var res = await adapter.GetSubmissionMandateAsync(bban);

            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "brunelatolivier@gmail.com");
            var company = new Company(Guid.Empty, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");
            var collection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2019, 10, 10, 8, 54, 3), new DateTime(2019, 10, 10, 8, 54, 3), status);

            res.Should().BeEquivalentTo(collection);

            formIoClient.VerifyAll();
        }

        [Fact]
        public async Task GetSubmissionMandateAsync_When_GetSubmissionMandateAsync_Return_NoResult()
        {
            Bban bban = new Bban("13507", "00014", "31464482121", "77", "8909440", null);
            var sub = new FormioSubmissionCollection()
            {
                Limit = 1,
                Skip = 0,
                Total = 1,
                Submissions = JsonConvert.DeserializeObject<List<FormioSubmission>>("[]") !,
            };

            var formIoClient = new Mock<IFormIoClient>(MockBehavior.Strict);
            formIoClient.Setup(item => item.GetSubmissionMandateAsync("13507", "00014", "31464482121", "77", It.IsAny<FormioAuthToken>()))
                .ReturnsAsync(sub)
                .Verifiable();

            var adapter = new FormIoAdapter(formIoClient.Object);

            var res = await adapter.GetSubmissionMandateAsync(bban);

            res.Should().BeNull();

            formIoClient.VerifyAll();
        }
    }
}
