using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Assessment_ADO
{
    class Customer
    {
        public int CustId  {get; set;}
        public int ProdID { get; set; }
        public int SuppID { get; set; }
        public string CustName { get; set; }
        public int Amount { get; set; }
        public float Total { get; set; }

    }

    class Program
    {
        SqlConnection con;
        SqlCommand cmd;
        string ConnString = "data source =IN5CG9214Y41; database = ADODemo ; integrated security = true";
        
        public void ReadProductsList()
        {
            con = new SqlConnection();
            con.ConnectionString = ConnString;
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_ReadProductList";
            cmd.Connection = con;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            Console.WriteLine("PRODUCT\t\tPRODUCT ID");
            Console.WriteLine("-------\t\t----------");
            while (rdr.Read())
            {
                Console.WriteLine($"{rdr[1]}\t\t{rdr[0]}");
            }
            rdr.Close();
            con.Close();

        }

        public void ReadSupplierById(int index)
        {
            con = new SqlConnection(); 
            con.ConnectionString = ConnString;
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_ReadSupplier";
            cmd.Parameters.AddWithValue("piD", index);

            cmd.Connection = con;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            Console.WriteLine("Product ID\t\tSupplier\t\tSupplier ID\t\tLocation\t\tPrice");
            Console.WriteLine("------- --\t\t--------\t\t-------- --\t\t--------\t\t-----");
            while (rdr.Read())
            {
                Console.WriteLine($"{rdr["prod_id"]}\t\t{rdr["CompanyName"]}\t\t{rdr["SpID"]}\t\t{rdr["sLocation"]}\t\t{rdr["Price"]}");
            }
            rdr.Close();
            con.Close();

        }

        public double GetPrice(int prodid, int supplierid)
        {
            object price ;

            con = new SqlConnection(); 
            con.ConnectionString = ConnString;
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_RetrievePrice";
            cmd.Parameters.AddWithValue("prodid", prodid);
            cmd.Parameters.AddWithValue("spid", supplierid);
            cmd.Connection = con;
            con.Open();
            // SqlDataReader rdr = cmd.ExecuteReader();
            // price =(int) rdr[];
            price = cmd.ExecuteScalar();
            con.Close();
            return(double) price;

        }

        public string GetProductName(int prodid)
        {
            con = new SqlConnection(); 
            con.ConnectionString =ConnString;
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_RetrieveProductName";
            cmd.Parameters.AddWithValue("pro", prodid);
            cmd.Connection = con;
            con.Open();
            string name = (string)cmd.ExecuteScalar();
            con.Close();
            return name;
        }

        public string GetSupplierName(int supplyid)
        {
            con = new SqlConnection(); 
            con.ConnectionString = ConnString;
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_RetrieveSuppName";
            cmd.Parameters.AddWithValue("supId", supplyid);
            cmd.Connection = con;
            con.Open();
            string name = (string)cmd.ExecuteScalar();
            con.Close();
            return name;
        }



        public void InsertNewCustomer()
        {
            //int count=0;
            con = new SqlConnection(); 
            con.ConnectionString = ConnString;
           SqlCommand  cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            Customer cust = new Customer();
            Console.WriteLine("Enter Customer ID : ");
            cust.CustId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Customer name :");
            cust.CustName = Console.ReadLine();

            Console.WriteLine("Select Product Id from list");
            ReadProductsList();
            cust.ProdID = int.Parse(Console.ReadLine());
            Console.WriteLine("Select desired Supplier ID");
            ReadSupplierById(cust.ProdID);
            cust.SuppID = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter amount of product needed :");
            cust.Amount = int.Parse(Console.ReadLine());
            int price = (int) GetPrice(cust.ProdID, cust.SuppID);
            cust.Total = cust.Amount * price;
           
            cmd.CommandText = "sp_InsertCustomer";
            cmd.Parameters.AddWithValue("@CId", cust.CustId);
            cmd.Parameters.AddWithValue("@pID", cust.ProdID);
            cmd.Parameters.AddWithValue("@sID", cust.SuppID);
            cmd.Parameters.AddWithValue("@CName", cust.CustName);
            cmd.Parameters.AddWithValue("@Amt", cust.Amount);
            cmd.Parameters.AddWithValue("@tot", cust.Total);

            cmd.Connection = con;
            con.Open();

            int rowcount = (int)cmd.ExecuteNonQuery();
            if (rowcount > 0)
            {
                Console.WriteLine("Record Inserted Successfully");
            }

            con.Close();

            // return count;
        }

        public void DisplayCustomersList()
        {
            con = new SqlConnection();
            con.ConnectionString = ConnString;
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.CommandText = "sp_ReadCustomerList";
            cmd.Connection = con;
            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Console.WriteLine($"{rdr[0]}\t\t{rdr[1]}\t\t{GetProductName((int)rdr[2])}\t\t{GetSupplierName((int)rdr[3])}\t\t{rdr[4]}\t\t{rdr[5]}");
            }

            con.Close();

        }
        public void DisplayBill(int xId)
        {
            con = new SqlConnection();
            con.ConnectionString = ConnString;
            cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_ReadCustomerById";
            cmd.Parameters.AddWithValue("id", xId);
            cmd.Connection = con;
            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine($"_______________BILL for {rdr["Name"]}_____________");
                Console.WriteLine($"Product  : {GetProductName((int)rdr[2])}******");
                Console.WriteLine($"Sold By  : {GetSupplierName((int)rdr[3])}*****");
                Console.WriteLine($"Quantity : {rdr["Quantity"]}*************");
                Console.WriteLine($"Price    : {GetPrice((int)rdr[2], (int)rdr[3])}******");
                Console.WriteLine($"Total    : {rdr[4]} X {GetPrice((int)rdr[2], (int)rdr[3])} = {rdr[5]}******");
                // Console.WriteLine($"{rdr[0]}\t\t{rdr[1]}\t\t{RetrieveProductName((int)rdr[2])}\t\t{RetrieveSupplierName((int)rdr[3])}\t\t{rdr[4]}\t\t{rdr[5]}");
            }

            con.Close();


        }

        public static void Main(string[] args)
        {
            Program p = new Program();
            int y = 0;
            int choice;
            do
            {
                Console.WriteLine("1. New Customer Entry \t\t2. All Customer Details\t\t3.Customer Bill by ID");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        p.InsertNewCustomer();
                        Console.ReadLine();
                        break;
                    case 2:
                        p.DisplayCustomersList();
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine("Enter Customer Id to view bill");
                        int id = int.Parse(Console.ReadLine());
                        p.DisplayBill(id);
                        Console.ReadLine();
                        break;
                    default: Console.WriteLine("invalid"); break;
                }

                Console.WriteLine("Enter 1 to Continue 0 to exit");
                y = int.Parse(Console.ReadLine());
                if (y == 0)
                    Environment.Exit(1);
            } while (y == 1);

            Console.ReadLine();

        }
    }
}