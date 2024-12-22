using System.Net.Http.Json;
using NorthwindWebAPI.Models;

var baseUrl = "http://localhost:5237/api/products";

using var client = new HttpClient();

// Get all products
// var newProduct = new Product
// {
//     ProductName = "New Product",
//     SupplierID = 1,
//     CategoryID = 1,
//     QuantityPerUnit = "10 boxes",
//     UnitPrice = 20.0M,
//     UnitsInStock = 100,
//     UnitsOnOrder = 0,
//     ReorderLevel = 10,
//     Discontinued = false
// };

while (true)
{
    Console.WriteLine("Choose an action:");
    Console.WriteLine("1. View all products");
    Console.WriteLine("2. Search product by name");
    Console.WriteLine("3. Create a new product");
    Console.WriteLine("4. Update an existing product");
    Console.WriteLine("5. Delete a product(Doesn't work)");
    Console.WriteLine("6. Exit");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            Console.WriteLine("Fetching all products...");
            var products = await client.GetFromJsonAsync<List<Product>>(baseUrl);
            if (products != null && products.Count > 0)
            {
                products.ForEach(p => Console.WriteLine($"{p.ProductID}: {p.ProductName}, Price: {p.UnitPrice}"));
            }
            else
            {
                Console.WriteLine("No products found.");
            }
            break;

        case "2":
            Console.WriteLine("Enter a product name to search:");
            var productName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(productName))
            {
                Console.WriteLine($"Searching for product with name: {productName}...");
                var productByName = await client.GetFromJsonAsync<List<Product>>($"{baseUrl}?name={productName}");
                if (productByName != null && productByName.Count > 0)
                {
                    productByName.ForEach(p => Console.WriteLine($"Found: {p.ProductID}: {p.ProductName}, Price: {p.UnitPrice}"));
                }
                else
                {
                    Console.WriteLine("No products found with the given name.");
                }
            }
            break;

        case "3":
            Console.WriteLine("Enter details for the new product:");
            var newProduct = new Product
            {
                ProductName = Prompt("Product Name: "),
                SupplierID = int.Parse(Prompt("Supplier ID: ")),
                CategoryID = int.Parse(Prompt("Category ID: ")),
                QuantityPerUnit = Prompt("Quantity Per Unit: "),
                UnitPrice = decimal.Parse(Prompt("Unit Price: ")),
                UnitsInStock = short.Parse(Prompt("Units In Stock: ")),
                UnitsOnOrder = short.Parse(Prompt("Units On Order: ")),
                ReorderLevel = short.Parse(Prompt("Reorder Level: ")),
                Discontinued = bool.Parse(Prompt("Discontinued (true/false): "))
            };

            Console.WriteLine("Creating a new product...");
            var createResponse = await client.PostAsJsonAsync(baseUrl, newProduct);
            if (createResponse.IsSuccessStatusCode)
            {
                var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();
                Console.WriteLine($"Product created: ID = {createdProduct?.ProductID}, Name = {createdProduct?.ProductName}");
            }
            else
            {
                Console.WriteLine($"Failed to create product. Status: {createResponse.StatusCode}");
            }
            break;

        case "4":
            var allProducts = await client.GetFromJsonAsync<List<Product>>(baseUrl);
            if (allProducts != null && allProducts.Count > 0)
            {
                Console.WriteLine("Select a product to update (by ID):");
                var productId = int.Parse(Console.ReadLine() ?? "0");
                var productToUpdate = allProducts.FirstOrDefault(p => p.ProductID == productId);

                if (productToUpdate != null)
                {
                    Console.WriteLine("Enter new details for the product (leave blank to keep current value):");
                    productToUpdate.ProductName = Prompt($"Product Name ({productToUpdate.ProductName}): ", productToUpdate.ProductName);
                    productToUpdate.SupplierID = int.Parse(Prompt($"Supplier ID ({productToUpdate.SupplierID}): ", productToUpdate.SupplierID.ToString()));
                    productToUpdate.CategoryID = int.Parse(Prompt($"Category ID ({productToUpdate.CategoryID}): ", productToUpdate.CategoryID.ToString()));
                    productToUpdate.QuantityPerUnit = Prompt($"Quantity Per Unit ({productToUpdate.QuantityPerUnit}): ", productToUpdate.QuantityPerUnit);
                    productToUpdate.UnitPrice = decimal.Parse(Prompt($"Unit Price ({productToUpdate.UnitPrice}): ", productToUpdate.UnitPrice.ToString()));
                    productToUpdate.UnitsInStock = short.Parse(Prompt($"Units In Stock ({productToUpdate.UnitsInStock}): ", productToUpdate.UnitsInStock.ToString()));
                    productToUpdate.UnitsOnOrder = short.Parse(Prompt($"Units On Order ({productToUpdate.UnitsOnOrder}): ", productToUpdate.UnitsOnOrder.ToString()));
                    productToUpdate.ReorderLevel = short.Parse(Prompt($"Reorder Level ({productToUpdate.ReorderLevel}): ", productToUpdate.ReorderLevel.ToString()));
                    productToUpdate.Discontinued = bool.Parse(Prompt($"Discontinued ({productToUpdate.Discontinued}): ", productToUpdate.Discontinued.ToString()));

                    Console.WriteLine("Updating a product...");
                    var updateResponse = await client.PutAsJsonAsync($"{baseUrl}/{productToUpdate.ProductID}", productToUpdate);
                    if (updateResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Product updated: ID = {productToUpdate.ProductID}, New Name = {productToUpdate.ProductName}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to update product. Status: {updateResponse.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine("Product not found.");
                }
            }
            break;

        // case "5":
        //     var productsToDelete = await client.GetFromJsonAsync<List<Product>>(baseUrl);
        //     if (productsToDelete != null && productsToDelete.Count > 0)
        //     {
        //         Console.WriteLine("Select a product to delete (by ID):");
        //         var deleteId = int.Parse(Console.ReadLine() ?? "0");
        //         var deleteResponse = await client.DeleteAsync($"{baseUrl}/{deleteId}");
        //         if (deleteResponse.IsSuccessStatusCode)
        //         {
        //             Console.WriteLine($"Product deleted: ID = {deleteId}");
        //         }
        //         else
        //         {
        //             Console.WriteLine($"Failed to delete product. Status: {deleteResponse.StatusCode}");
        //         }
        //     }
        //     else
        //     {
        //         Console.WriteLine("No products available to delete.");
        //     }
        //     break;

        case "6":
            Console.WriteLine("Exiting application...");
            return;

        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
}


// Helper method for prompting user input
string Prompt(string message, string defaultValue = "")
{
    Console.Write(message);
    var input = Console.ReadLine();
    return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
}