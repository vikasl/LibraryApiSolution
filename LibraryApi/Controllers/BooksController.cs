using AutoMapper;
using LibraryApi.Domain;
using LibraryApi.Models.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace LibraryApi.Controllers
{
    public class BooksController : ControllerBase
    {

        LibraryDataContext _context;
        IMapper _mapper;
        MapperConfiguration _config;

        public BooksController(LibraryDataContext context, IMapper mapper, MapperConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }





        // GET /books - returns a collection of all our books.
        //            - can filter on genre
        [HttpGet("books")]
        [Produces("application/json")]
        public async Task<ActionResult<GetBooksResponse>> GetAllBooks([FromQuery] string genre ="All")
        {
            var response = new GetBooksResponse();

            var results = _context.Books

                .Where(b => b.RemovedFromInventory == false);

            if (genre != "All")

            {

                results = results.Where(b => b.Genre == genre);

            }

            response.Data = await results.ProjectTo<GetBooksResponseItem>(_config).ToListAsync();


            response.Genre = genre;
            response.Count = response.Data.Count;
            return Ok(response); //TODO THIS SUCKS FIX THIS!
        }

        /// <summary>
        /// Retrieve a single book
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        // GET /books/{id} - get a single book or a 404
        [HttpGet("books/{bookId:int}", Name = "books#getbookbyid")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetBookDetailsResponse>> GetBookById(int bookId)
        {
            var response = await _context.Books.Where(b => b.RemovedFromInventory == false && b.Id == bookId)
                 .ProjectTo<GetBookDetailsResponse>(_config)
                 .SingleOrDefaultAsync();

            if( response == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(response);
            }

        }

       

        // POST /books - Add a document to the collection

        // DELETE /books/{id} - remove a book from inventory
        [HttpDelete("books/{bookId:int}")]
        public async Task<ActionResult> RemoveBookFromInventory(int bookId)
        {
            var book = await _context.Books.SingleOrDefaultAsync(b => b.Id == bookId && b.RemovedFromInventory == false);
            if(book != null)
            {
                book.RemovedFromInventory = true;
                await _context.SaveChangesAsync();
               
            }
            return NoContent(); //Fine
        }
        [HttpPost("books")]
        public async Task<ActionResult> AddABook([FromBody] BookCreateRequest bookToAdd)
        {
            //1. validate the incoming entity
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var book = _mapper.Map<Book>(bookToAdd);
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                var response = _mapper.Map<GetBookDetailsResponse>(book);

                return CreatedAtRoute("books#getbookbyid", new { bookId = response.Id });
            }


            //if it fails send back a 400 with or without details
            // 2. Change the domain
            // - add the book to the database.( BookCreateRequest -> Book)
            // 3. Return
            //    201 Created
            // Location Header ( what is the name of the new book  - name being the URI
            //


            //return Ok();
        }

        // PUT /books/{id} - update a book (We'll talk a bunch about this when we get to it.)

        [HttpPut("books/{bookId:int}/genre")]
        public async Task<ActionResult> UpdateGenre(int bookId, [FromBody] string newGenre)
        {
            var book = await _context.Books.Where(b => b.Id == bookId).SingleOrDefaultAsync();
            if(book == null)
            {
                return NotFound();
            }
            else
            {
                book.Genre = newGenre;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

      
        [HttpPut("books/{id}")]
        public async Task<ActionResult> UpdateBooks(int id, [FromBody] GetBookDetailsResponse book)
        {
            var storedBook = await _context.Books.Where(b => b.Id == id && b.RemovedFromInventory == false).SingleOrDefaultAsync();
            if (storedBook != null)
            {
                storedBook.Title = book.Title;
                storedBook.Author = book.Author;
                storedBook.Genre = book.Genre;
                await _context.SaveChangesAsync();
                return NoContent();


            }
            else
            {
                // ?? "Upsert"
                return NotFound();
            }
        }




    }
}
