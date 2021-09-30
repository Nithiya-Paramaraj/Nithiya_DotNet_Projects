using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;


namespace Portfolio_berechnen


{    

   class Program

        {

        private static void Main(string[] args)

        {
            string givenInvestorId;
            string givenStichtag;
            double FinalImmobilienWert = 0;
            double FinalFondsWert = 0;
            double FinalAktieWert = 0;
            int NoOfInvestedShares = 200;
            int ValueOfTheFund = 20000000;
            

            string inputInvestmentCSVFileName = @"/Users/DOT_NET_Projects/QPlix_Code_Test/Investments.csv";
            string inputTransactionsCSVFileName = @"/Users/DOT_NET_Projects/QPlix_Code_Test/Transactions.csv";
            string inputQuotesCSVFileName = @"/Users/DOT_NET_Projects/QPlix_Code_Test/Quotes.csv";

            string[] dateFormats = new[] { "mm.dd.yy","yyyy/MM/dd", "MM/dd/yyyy", "MM/dd/yyyyHH:mm:ss"};
            CultureInfo provider = new CultureInfo("en-US");

            List<Investment> listOfInvestments = new List<Investment>();
            List<Quotes> listOfQuotes = new List<Quotes>();
            List<Transactions> listOfTransactions = new List<Transactions>();

            try
            {
                Console.WriteLine("Bitte geben Sie InvestorId & Datum durch Semikolon getrennt ein  , z.B Investor1, 15.01.16");
                string line = Console.ReadLine();

                if (String.IsNullOrEmpty(line))
                {
                    Console.WriteLine("Bitte geben Sie InvestorID & Datum ein: ");
                }
                else
                {
                    string[] Inputvalues = line.Split(", ");

                    givenInvestorId = Inputvalues[0];
                    givenStichtag = Inputvalues[1];
                    //DateTime givenStichtag = DateTime.ParseExact(Inputvalues[1], dateFormats,provider, DateTimeStyles.AdjustToUniversal);



                    //Read data from Investments csv file
                    using (var investment_File_Reader = new StreamReader(inputInvestmentCSVFileName))
                    {
                        while (!investment_File_Reader.EndOfStream)
                        {
                            var investment_File_Read = investment_File_Reader.ReadLine();
                            var values = investment_File_Read.Split(';');
                            if (givenInvestorId == values[0])
                            {
                                //Add the matched Investor ID
                                listOfInvestments.Add(new Investment(values[0], values[1], values[2], values[3], values[5]));

                            }
                        }

                    }//End of reading transactions File 

                                        
                    //Read data from Transactions File
                    using (var transaction_File_Reader = new StreamReader(inputTransactionsCSVFileName))
                    {
                        while (!transaction_File_Reader.EndOfStream)
                        {
                            var transaction_File_Read = transaction_File_Reader.ReadLine();
                            var values = transaction_File_Read.Split(';');

                            for (int investmentRow = 0; investmentRow < listOfInvestments.Count; investmentRow++)
                            {

                                if (listOfInvestments[investmentRow].InvestmentId==values[0])
                                
                                {
                                    listOfTransactions.Add(new Transactions(values[0], values[1], values[2], Convert.ToDouble(values[3])));

                                }
                            
                            }

                        }// End of while
                    }//End of using transactions File Reading



                    // Read data from Quotes CSV File
                    using (var quotes_File_Reader = new StreamReader(inputQuotesCSVFileName))
                    {
                        while (!quotes_File_Reader.EndOfStream)
                        {
                            var quotes_File_Read = quotes_File_Reader.ReadLine();
                            var values = quotes_File_Read.Split(';');
                            for (int investmentRow = 0; investmentRow < listOfInvestments.Count; investmentRow++)
                            {
                                if (listOfInvestments[investmentRow].InvestmentISIN == values[0])

                                {
                                    listOfQuotes.Add(new Quotes(values[0], values[1], Convert.ToDouble(values[2])));
                                }

                            }
                            
                        }
                    }// End of reading Quotes File

                                       
                    //Calculate the Werte for the matched InvestorID & InvestmentType
                    for (int investmentRow = 0; investmentRow < listOfInvestments.Count; investmentRow++)
                    {
                        for (int transactionRow = 0; transactionRow < listOfTransactions.Count; transactionRow++)
                        {

                            if (listOfInvestments[investmentRow].InvestmentId == listOfTransactions[transactionRow].InvestmentID)
                            {

                                if (listOfInvestments[investmentRow].InvestmentType == "RealEstate")
                                {
                                    FinalImmobilienWert = FinalImmobilienWert + Convert.ToDouble(listOfTransactions[transactionRow].TransactionValue);
                                }


                                if (listOfInvestments[investmentRow].InvestmentType == "Stock")
                                {
                                    for (int sharepricerow = 0; sharepricerow < listOfQuotes.Count; sharepricerow++)
                                    {
                                        if (listOfQuotes[sharepricerow].InvestmentISIN == listOfInvestments[investmentRow].InvestmentISIN)
                                        {
                                            if (listOfQuotes[sharepricerow].QuotesDate == givenStichtag)
                                            {
                                                //No of invested shares is hardcoded as 200
                                                FinalAktieWert = FinalAktieWert + (NoOfInvestedShares * Convert.ToDouble(listOfQuotes[sharepricerow].PricePerShare));
                                            }
                                        }
                                    }
                                }
                                
                                //var insert = DateTime.ParseExact(givenStichtag, "mm.dd.yy h:mm", CultureInfo.InvariantCulture);
                                //var insert2 = DateTime.ParseExact(values[2], "mm.dd.yy h:mm", CultureInfo.InvariantCulture);

                                string newgivenStichTag = givenStichtag.Replace(".", "-");

                                string strDate = newgivenStichTag;
                                string[] dateString = strDate.Split('-');
                                DateTime enter_date = Convert.ToDateTime(dateString[1] + "/" + dateString[0] + "/" + dateString[2]);

                                //if (Convert.ToDateTime(listOfTransactions[transactionRow].TransactionDate) == enter_date)

                                {

                                    if (listOfInvestments[investmentRow].InvestmentType == "Fonds")
                                    {
                                        //Value of the Fund is hardcoded as 2 Million
                                        FinalFondsWert = FinalFondsWert + (ValueOfTheFund * listOfTransactions[transactionRow].TransactionValue);//Value of Fund * % Invested from Transactions File
                                    }
                                    
                                }

                            }// End of If list of investments

                        }// End of for


                        Console.WriteLine("Wert des Portfolios ist:  " + (FinalImmobilienWert + FinalFondsWert + FinalAktieWert));

                        Console.ReadLine();
                    }// End of if else input null               
                }// End of try catch
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has occured:  Bitte geben Sie InvestorId & Datum in die Richtige Format:  " + "Error Message: " + ex.Message);
            }
        }//End of Main method
    }//End of class

    class Investment
    {

        private string investorId;
        private string investmentId;
        private string investmentType;
        private string investmentISIN;
        private string investmentFondsInvestor;

        public Investment(string investorId, string investmentId, string investmentType, string investmentISIN, string investmentFondsInvestor )
        {
            this.investorId = investorId;
            this.investmentId = investmentId;
            this.investmentType = investmentType;
            this.investmentISIN = investmentISIN;
            this.investmentFondsInvestor = investmentFondsInvestor;
        }


        public string InvestorId

        {
            get { return investorId; }

            set { investorId=value; }
        }

        public string InvestmentId
        {
            get { return investmentId; }

            set { investmentId = value; }
        }

        public string InvestmentType
        {
            get { return investmentType; }

            set { investmentType = value; }
        }

        public string InvestmentISIN
        {
            get { return investmentISIN; }

            set { investmentISIN = value; }
        }

        public string InvestmentFondsInvestor
        {
            get { return investmentFondsInvestor; }

            set { investmentFondsInvestor = value; }
        }

    }//End of Class Investment

    class Quotes
    {

        private string investmentISIN;
        private string quotesDate;
        private double pricePerShare;


        public Quotes(string investmentISIN, string quotesDate, double pricePerShare)
        {
            this.investmentISIN = investmentISIN;
            this.quotesDate = quotesDate;
            this.pricePerShare = pricePerShare;
        }

        public string InvestmentISIN

        {
            get { return investmentISIN; }

            set { investmentISIN = value; }
        }

        public string QuotesDate
        {
            get { return quotesDate; }

            set { quotesDate = value; }
        }

        public double PricePerShare
        {
            get { return pricePerShare; }

            set { pricePerShare = value; }
            
        }
    }// End of Class Quotes

    class Transactions
    {

        private string investmentID;
        private string transactionType;
        private string transactionDate;
        private double transactionValue;


        public Transactions(string investmentID, string transactionType, string transactionDate, double transactionValue)
        {
            this.investmentID = investmentID;
            this.transactionType = transactionType;
            this.transactionDate = transactionDate;
            this.transactionValue = transactionValue;
        }

        public string InvestmentID

        {
            get { return investmentID; }

            set { investmentID = value; }
        }

        public string TransactionType
        {
            get { return transactionType; }

            set { transactionType = value; }
        }

        public string TransactionDate
        {
            get { return transactionDate; }

            set { transactionDate = value; }

        }
        public double TransactionValue
        {
            get { return transactionValue; }

            set { transactionValue = value; }

        }
    }// End of Class Transactions

}//End of namespace

