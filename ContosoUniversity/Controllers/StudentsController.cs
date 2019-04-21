using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        /*
            NOTE: The scaffolding engine can also create the database context for you if you don't create it manually first as you did earlier for this tutorial. You can specify a new context class in the Add Controller box by clicking the plus sign to the right of Data context class. Visual Studio will then create your DbContext class as well as the controller and views.)
             */
        private readonly SchoolContext _context;
        //NOTE: The dependency injection that you configured in Startup.cs takes care of passing an instance of SchoolContext into the controller.
        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        //=======================================================================================


        // GET: Students
        public async Task<IActionResult> Index()
        {
            //NOTE: The Index method gets a list of students from the Students entity set by reading the Students property of the database context instance:
            return View(await _context.Students.ToListAsync());
            //NOTE: The Views/Students/Index.cshtml view displays the student list in a table. The "async" keyword tells the compiler to generate callbacks for parts of the method body and to automatically create the Task<IActionResult> object that's returned.  The "await" keyword causes the compiler to split the method into two parts. The first part ends with the operation that's started asynchronously. The second part is put into a callback method that's called when the operation completes. The return type Tast<IActionResult> represents ongoing work with a result of type IActionResult. ToListAsync is the asynchronous version of the ToList extension method.

            //NOTE: When writing async code in Entity, only statements that cause queries or commands to be sent to the database are executed asynchronously. That includes ToListAsync, SingleOrDefaultAsync, and SaveChangesAsync. By contrast, statements that just change an IQueryable, such as var students = context.Students.Where(s => s.LastName == "something" do not.
        }

        //=======================================================================================

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //NOTE: The key value that's passed in to the Details method comes from route data. This is data that the model binder found in a segment of the URL. It specifies the controller, action, and id segments. This is found in Startup.cs (" app.UseMvc..." ).
            //NOTE: The controller is HomeController.cs, action is Index, and the id is the given id. The last part of the URL (?courseID=456) is a query string value. an example of the URL for this is: localhost/Home/Index/1?courseID=456. In the Details method, "id" is passed in as a query string and "courseID=456" could be a second parameter.
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            //NOTE: FirstOrDefaultAsync asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements. 
            //NOTE: SingleOrDefaultAsync asynchronously returns only the element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition. Here, it returns a single Student object. Also, the "Include", ".ThenInclude" and "AsNoTracking" methods are added. Include and .ThenInclude cause the context to laod the Student.Enrollments navigation property (remember, navigation properties hint at foreign keys) and within each enrollment the Enrollment.Course navigation property. AsNoTracking improves performance in scenarios where the entities returned won't be updatd in the current context's lifetime.
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        //=======================================================================================


        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        //NOTE: The "Id" was removed from the Bind attribute and a try-catch block was added to the Create method. A model binder refers to the ASP.NET Core MVC functionality that makes it easier for you to work with data submitted by a form. You removed the ID from the Bind attribute because the ID is the primary key value which SQL Server will set automatically when the row is inserted. This is a form - input by the user doesn't set the ID value. For a more in-depth explanation of Bind and how it has a protective effect, see this link: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud?view=aspnetcore-2.2


        public async Task<IActionResult> Create(
    [Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
        {
            try
            {
                if (ModelState.IsValid) //NOTE: This is the form validation
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(student);
        }

        //=======================================================================================


        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        //NOTE: The code above is the boilerplate edit code. The following shows the best practice to prevent overposting (see link in Details section related to "Bind") and maintain security. It ensures that only changed columns get updated and preserves data in properties that you don't want included for model binding.

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        //NOTE: See also the alternative HttpPost Edit code (Create and attach) in this link: ( https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud?view=aspnetcore-2.2 ). The scaffolded code above also uses the create-and-attach approach but only catches DbUpdateConcurrencyException exceptions and returns 404 error codes. The example shown catches any database update exception and displays an error message.

        //=======================================================================================


        //=======================================================================================


        // GET: Students/Delete/5

        //NOTE: The Delete method requires two operations, as for update and create. The GET request method displays a view that gives the user a chance to approve or cancel the delete operation. If the user approves, the POST request is made and the HttpPost Delete method is called. That is the method that actually performs the delete operation.

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        //=======================================================================================


        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}
