// <copyright file="FormTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Mongo.Tests
{
    using Newtonsoft.Json;

    public class FormTest
    {
        [Fact]
        public void Deserialization()
        {
            var mongoReference = @"
[
    {
        ""_id"": ""6481985db941fc7e7cd24b5a"",
        ""owner"": ""5d2728a785af96014dbd1b7e"",
        ""roles"": [],
        ""_vid"": 0,
        ""_fvid"": 10,
        ""state"": ""submitted"",
        ""data"": {
            ""accountNumber"": ""1000871602"",
            ""companyName"": ""SAS Madeleine Bike Sport"",
            ""SIRETNumber"": ""91211713200015"",
            ""signatoryTitle"": ""M"",
            ""signatoryLastName"": ""BARTIER"",
            ""signatoryFirstName"": ""OLIVIER"",
            ""headOffice"": {
                ""signatoryStreetAddress"": ""6 RUE DE FLORIADE"",
                ""signatoryAddressComplements"": ""comp"",
                ""signatoryAddressZipCode"": ""59890"",
                ""signatoryAddressCity"": ""QUESNOY SUR DEULE"",
                ""signatoryAddressCountry"": ""FRANCE""
            },
            ""panelStatusHistory"": [
                {}
            ],
            ""ebicsStatus"": ""ebics"",
            ""jdcRibId"": ""12358349"",
            ""jdcReleveId"": ""12358349"",
            ""bankSortCode"": ""03404"",
            ""bankCode"": ""30004"",
            ""jdcDossierId"": ""23562616"",
            ""companyId"": ""cid"",
            ""bankAccountNumber"": ""00010172049"",
            ""bankCheckNumber"": ""69""
        },
        ""access"": [],
        ""form"": ""5d27359685af9665b6bd1bb3"",
        ""project"": ""5d27359385af967188bd1b9e"",
        ""externalIds"": [],
        ""created"": ""2023-06-08T08:59:09.231Z"",
        ""modified"": ""2023-06-12T07:16:59.944Z"",
        ""metadata"": {
            ""timezone"": ""Europe/Paris"",
            ""offset"": 120,
            ""referrer"": ""https://esc.kpmg.fr/"",
            ""browserName"": ""Netscape"",
            ""userAgent"": ""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36"",
            ""pathName"": ""/jdc/mandat/6481985db941fc7e7cd24b5a"",
            ""onLine"": true
        }
    }
]
";

            List<Form> document = JsonConvert.DeserializeObject<List<Form>>(mongoReference)!;

            document.Should().NotBeNull();
            document.Should().HaveCount(1);
            document[0].Should().NotBeNull();
            document[0].Created.Should().Be("2023-06-08T08:59:09.231Z");
            document[0].Modified.Should().Be("2023-06-12T07:16:59.944Z");
            document[0].Data.Should().NotBeNull();
            document[0].Data!.AccountNumber.Should().Be("1000871602");

            // TODO : other properties
        }
    }
}
