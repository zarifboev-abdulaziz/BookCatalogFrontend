using LibraryManagementFront.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryManagementFront.Controllers
{
    public class BookController : Controller
    {
        public string BASEURL = "http://ec2-18-194-186-54.eu-central-1.compute.amazonaws.com/";

        public string BASEURL_local = "http://localhost:8000/";

        // GET: BookController
        public async Task<ActionResult> Index()
        {
            List<Book> books = new List<Book>();

            using (var client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request
                HttpResponseMessage response = await client.GetAsync("api/Books");

                if (response.IsSuccessStatusCode)
                {
                    //Storing the response
                    var rawResponse = response.Content.ReadAsStringAsync().Result;

                    books = JsonConvert.DeserializeObject<List<Book>>(rawResponse);
                }

            }

            return View(books);
        }


        // GET: BookController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Book book = new Book();

            using (var client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request
                HttpResponseMessage response = await client.GetAsync("api/Books/" + id.ToString());

                if (response.IsSuccessStatusCode)
                {
                    //Storing the response
                    var rawResponse = response.Content.ReadAsStringAsync().Result;

                    book = JsonConvert.DeserializeObject<Book>(rawResponse);
                }

            }

            return View(book);
        }



        // GET: BookController/Create
        public async Task<ActionResult> Create()
        {
            // Fetch categories from the API or your database
            List<Category> categories = new List<Category>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/Categories");

                if (response.IsSuccessStatusCode)
                {
                    var rawResponse = await response.Content.ReadAsStringAsync();
                    categories = JsonConvert.DeserializeObject<List<Category>>(rawResponse);
                }
            }

            // Prepare a SelectList for the categories
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Book book)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(book);
                Console.WriteLine("Raw JSON Response:");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("api/Books", content);
                Console.WriteLine(response.Content);


                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Unable to create book. Please try again.");
                }
            }

            // If we got this far, something failed; redisplay the form
            return View(book);
        }

        // GET: BookController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Book book = null;
            List<Category> categories = new List<Category>();

            using (var client = new HttpClient())
            {
                // Passing service base url
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();

                // Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Fetch the book details
                HttpResponseMessage bookResponse = await client.GetAsync($"api/Books/{id}");
                if (bookResponse.IsSuccessStatusCode)
                {
                    var rawResponse = await bookResponse.Content.ReadAsStringAsync();
                    book = JsonConvert.DeserializeObject<Book>(rawResponse);
                }

                // Fetch the list of categories
                HttpResponseMessage categoryResponse = await client.GetAsync("api/Categories"); // Adjust the endpoint as necessary
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var rawCategoryResponse = await categoryResponse.Content.ReadAsStringAsync();
                    categories = JsonConvert.DeserializeObject<List<Category>>(rawCategoryResponse);
                }
            }

            // Check if the book was found
            if (book == null)
            {
                return NotFound(); // Or redirect to an appropriate error page
            }

            // Pass the book and categories to the view
            ViewBag.Categories = new SelectList(categories, "Id", "Name"); // Assuming 'Id' is the category ID and 'Name' is the category name
            return View(book);
        }


        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Book book) // Change to accept Book model
        {
            using (var client = new HttpClient())
            {
                // Passing service base url
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();

                // Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Serialize the book object to JSON
                var json = JsonConvert.SerializeObject(book);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Sending PUT request to update the book
                HttpResponseMessage response = await client.PutAsync($"api/Books/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    // Redirect to index or another action upon success
                    return RedirectToAction(nameof(Index));
                }
            }

            // If we got this far, something failed, redisplay the form
            return View(book);
        }

        // GET: BookController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            Book book = null;

            using (var client = new HttpClient())
            {
                // Passing service base url
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();

                // Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Fetch the book details to confirm deletion
                HttpResponseMessage response = await client.GetAsync($"api/Books/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var rawResponse = await response.Content.ReadAsStringAsync();
                    book = JsonConvert.DeserializeObject<Book>(rawResponse);
                }
            }

            // Check if the book was found
            if (book == null)
            {
                return NotFound(); // Or redirect to an appropriate error page
            }

            // Pass the book object to the view
            return View(book);
        }


        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            using (var client = new HttpClient())
            {
                // Passing service base url
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Clear();

                // Sending DELETE request to remove the book
                HttpResponseMessage response = await client.DeleteAsync($"api/Books/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Redirect to index or another action upon success
                    return RedirectToAction(nameof(Index));
                }
            }

            // If deletion fails, redisplay the confirmation view
            return View();
        }
    }
}
