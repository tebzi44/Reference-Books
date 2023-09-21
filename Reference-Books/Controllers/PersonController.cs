using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reference_Books.Data;
using Reference_Books.Models.Domein;
using Reference_Books.Models.Dtos;
using Reference_Books.Models.Enums;

namespace Reference_Books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ReferenceBookDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<PersonController> logger;
        public PersonController(ReferenceBookDbContext dbContext, IWebHostEnvironment webHostEnvironment, ILogger<PersonController> logger)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> getAll()
        {
            try
            {
                var all = dbContext.Person.Include(p => p.Relations);
                return Ok(all);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-person/{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            try
            {
                var person = await dbContext.Person.Include(p => p.Relations).FirstOrDefaultAsync(p => p.Id == id);
                //if (person == null) { return NotFound(); }
                var imageUrl = person.PhotoUrl;

                    string filePath = Path.Combine(webHostEnvironment.WebRootPath) + imageUrl; //+
                if (imageUrl != null)
                {
                    byte[] imageData = System.IO.File.ReadAllBytes(filePath);

                    var image = File(imageData, "image/png");
                    return Ok(new { person, image });
                }
                return Ok(person);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");    //Here add!
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search-person")]
        public async Task<IActionResult> SearchPerson([FromQuery] string searchKey)
        {
            try
            {
                searchKey = searchKey.ToLower();

                var result = dbContext.Person
                    .Where(p => p.FirstName.ToLower() == searchKey ||
                                p.LastName.ToLower() == searchKey || 
                                p.PersonalNumber == searchKey);
                if(result.Count() == 0) { return NotFound("Not Found!"); }

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while searching a person.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-by-paging")]
        public async Task<IActionResult> GetByPaging([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                int SkippedData = (page - 1) * pageSize;
                var result = await dbContext.Person.Include(p => p.Relations).Skip(SkippedData).Take(pageSize).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-connection-report")]
        public async Task<IActionResult> GetConnectionReport()
        {
            try
            {
                var connectionCounts = await dbContext.Person // -1-
                .Include(p => p.Relations)
                .Select(person => new ConnectionReportDto
                (
                    person.Id,
                    person.Relations.Count(r => r.RelationType == RelationTypeEnum.Friend),
                    person.Relations.Count(r => r.RelationType == RelationTypeEnum.Colleague),
                    person.Relations.Count(r => r.RelationType == RelationTypeEnum.Relative)
                ))
                .ToListAsync();

                return Ok(connectionCounts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("add-person")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddPerson([FromForm] AddPlayerDto addDto, IFormFileCollection uploadedFiles)
        {
            try
            {
                if (uploadedFiles.Count != 1)
                {
                    return BadRequest("Only upload one Image!");
                }

                var person = new Person
                {
                    FirstName = addDto.FirstName,
                    LastName = addDto.LastName,
                    PersonalNumber = addDto.PersonalNumber,
                    BirthDay = addDto.BirthDay,
                    Gender = addDto.Gender,
                    City = addDto.City,
                };

                dbContext.Person.Add(person);
                await dbContext.SaveChangesAsync();

                if (uploadedFiles.Count == 1)
                {
                    var source = uploadedFiles[0];

                    string imageName = $"{person.Id}-image.png";
                    string filePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", person.Id.ToString());

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    string[] filesInDirectory = Directory.GetFiles(filePath);
                    if (filesInDirectory.Length > 0)
                    {
                        foreach (var files in Directory.GetFiles(filePath))
                        {
                            System.IO.File.Delete(files);
                        }
                    }

                    string imagePath = Path.Combine(filePath, imageName);
                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await source.CopyToAsync(stream);
                    }
                    //person.PhotoUrl = imagePath;
                    person.PhotoUrl = Path.Combine("\\Images\\", person.Id.ToString(), imageName);

                }

                if (addDto.PhoneNumbers != null)
                {
                    dbContext.PhoneNumber.Add(
                            new PhoneNumber
                            {
                                PersonId = person.Id,
                                MobileNumber = addDto.PhoneNumbers.MobileNumber,
                                HomeNumber = addDto.PhoneNumbers.HomeNumber,
                                OfficeNumber = addDto.PhoneNumbers.OfficeNumber
                            }
                        );
                }

                if (addDto.RelatedPeoplesList != null)
                {
                    foreach (var item in addDto.RelatedPeoplesList)
                    {
                        if (item != null)
                        {
                            dbContext.Relations.Add(
                                new Relation
                                {
                                    PersonId = person.Id,
                                    TargetPerson = item.TargetPerson,
                                    RelationType = item.RelationType
                                }
                            );
                        }
                    }
                }
                await dbContext.SaveChangesAsync();
                return Ok(person);
            } 
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update-person/{id}")]
        public async Task<IActionResult> UpdatePerson([FromRoute] int id, [FromBody] UpdatePersonDto updateDto)
        {
            try 
            { 
                var person = await dbContext.Person.FindAsync(id);
                if (person == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(updateDto.FirstName))
                {
                    person.FirstName = updateDto.FirstName;
                }

                if (!string.IsNullOrEmpty(updateDto.LastName))
                {
                    person.LastName = updateDto.LastName;
                }

                if (!string.IsNullOrEmpty(updateDto.PersonalNumber))
                {
                    person.PersonalNumber = updateDto.PersonalNumber;
                }

                if (updateDto.Gender.HasValue)
                {
                    person.Gender = updateDto.Gender.Value;
                }

                if (updateDto.BirthDay.HasValue)
                {
                    person.BirthDay = updateDto.BirthDay.Value;
                }

                if (updateDto.City.HasValue)
                {
                    person.City = updateDto.City.Value;
                }

                var phoneNumber = await dbContext.PhoneNumber.FirstOrDefaultAsync(p => p.PersonId == person.Id);

                if (phoneNumber != null && updateDto.PhoneNumbers != null)
                {
                    if (!string.IsNullOrEmpty(updateDto.PhoneNumbers.MobileNumber))
                    {
                        phoneNumber.MobileNumber = updateDto.PhoneNumbers.MobileNumber;
                    }

                    if (!string.IsNullOrEmpty(updateDto.PhoneNumbers.HomeNumber))
                    {
                        phoneNumber.HomeNumber = updateDto.PhoneNumbers.HomeNumber;
                    }

                    if (!string.IsNullOrEmpty(updateDto.PhoneNumbers.OfficeNumber))
                    {
                        phoneNumber.OfficeNumber = updateDto.PhoneNumbers.OfficeNumber;
                    }
                }

                await dbContext.SaveChangesAsync();
                return Ok(person);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update-image/{id}")]
        public async Task<IActionResult> UpdateIamge([FromForm] ImageUploadDto imageUploadDto)
        {
            try
            {
                var person = await dbContext.Person.FirstOrDefaultAsync(p => p.Id == imageUploadDto.Id); //dbContext.Person.FindAsync(id); test it!
                if (person == null)
                {
                    return NotFound(new { mess= "User not found!" });
                }

                var oldImageUrl = person.PhotoUrl;

                var source = imageUploadDto.Image;
                if (source == null)
                {
                    return BadRequest("No image uploaded!");
                }
                string directoryPath;

                if (!string.IsNullOrEmpty(oldImageUrl))
                {
                    directoryPath = Path.GetDirectoryName(webHostEnvironment.WebRootPath + oldImageUrl);

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    if (System.IO.File.Exists(webHostEnvironment.WebRootPath + oldImageUrl))
                    {
                        string tempImageName = $"\\{person.Id}-temp-image.png";
                        string tempFilePath = directoryPath + "\\temp" + tempImageName;

                        if (!Directory.Exists(directoryPath + "\\temp")) Directory.CreateDirectory(directoryPath + "\\temp");

                        await Task.Run(() => System.IO.File.Move(webHostEnvironment.WebRootPath + oldImageUrl, tempFilePath));
                        System.IO.File.Delete(webHostEnvironment.WebRootPath + oldImageUrl);
                    }
                }
                else
                {
                    directoryPath = Path.Combine(webHostEnvironment.WebRootPath, "Images", person.Id.ToString());
                }

                string imageName = $"\\{person.Id}-image.png";
                string newImageUrl = "\\Images\\" + person.Id + imageName;

                using (FileStream stream = System.IO.File.Create(directoryPath + "\\" + imageName))
                {
                    await source.CopyToAsync(stream);
                }

                person.PhotoUrl = newImageUrl;

                if (System.IO.File.Exists(webHostEnvironment.WebRootPath + $"\\Images\\{person.Id}\\temp\\{person.Id}-temp-image.png"))
                {
                   Directory.Delete(webHostEnvironment.WebRootPath + $"\\Images\\{person.Id}\\temp", true);
                }

                await dbContext.SaveChangesAsync();

                return Ok(webHostEnvironment.WebRootPath + $"image\\{person.Id}\\term\\{person.Id}-term-image.png");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update-relation/{id}")]
        public async Task<IActionResult> UpdateRelation([FromRoute]int id, [FromBody] List<UpdateRelationDto> updateRelationDtos)
        {
            try
            {
                var person = await dbContext.Person.Include(p => p.Relations).FirstOrDefaultAsync(p => p.Id == id);
                if (person == null)
                {
                    return NotFound();
                } 
                foreach (var updateDto in updateRelationDtos)
                {

                    if (updateDto.Action == UpdateRelationAction.Add)
                    {
                        var existingRelation = person.Relations.FirstOrDefault(r => r.TargetPerson == updateDto.TargetPerson);

                        if (existingRelation != null)
                        {
                            existingRelation.RelationType = updateDto.RelationType;
                        }
                        else
                        {
                            var newRelation = new Relation
                            {
                                TargetPerson = updateDto.TargetPerson,
                                RelationType = updateDto.RelationType
                            };
                            person.Relations.Add(newRelation);
                        }
                    }
                    else if (updateDto.Action == UpdateRelationAction.Delete)
                    {
                        var relationToRemove = person.Relations.FirstOrDefault(r => r.TargetPerson == updateDto.TargetPerson);

                        if (relationToRemove != null)
                        {
                            person.Relations.Remove(relationToRemove);
                        }
                        else
                        {
                            return NotFound("Target person not found!");
                        }
                    }
                    else
                    {
                        return BadRequest("Invalid action.");
                    }
                }

                await dbContext.SaveChangesAsync();
                return Ok(new {msg = person});
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-person/{id}")]
        public async Task<IActionResult> DeletePerson([FromRoute] int id)
        {
            try
            {
                var person = await dbContext.Person.FirstOrDefaultAsync(p => p.Id == id);

                if (person == null)
                {
                    return NotFound("User not found!");
                }

                dbContext.PhoneNumber.RemoveRange(dbContext.PhoneNumber.Where(pn => pn.PersonId == id));
                dbContext.Relations.RemoveRange(dbContext.Relations.Where(re => re.PersonId == id));
                dbContext.Person.Remove(person);
                await dbContext.SaveChangesAsync();

                return Ok("Person and related data deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
                return BadRequest(ex.Message);
            }
        }
    }
}





























// -1-
//var connectionCounts = await dbContext.Person
//.Include(p => p.Relations)
//.Select(person => new ConnectionReportDto
//{
//        PersonId = person.Id,
//        FriendsCount = person.Relations.Count(r => r.RelationType == RelationTypeEnum.Friend),
//        ColleaguesCount = person.Relations.Count(r => r.RelationType == RelationTypeEnum.Colleague),
//        RelativesCount = person.Relations.Count(r => r.RelationType == RelationTypeEnum.Relative)
//})
//.ToListAsync();









//[HttpPut("updateperson/{id}")]
//public async Task<IActionResult> UpdatePerson([FromRoute] int id, [FromBody] UpdatePersonDto updateDto)
//{
//    var person = await dbContext.Person.FindAsync(id);
//    if (person == null)
//    {
//        return NotFound();
//    }

//    if (!string.IsNullOrEmpty(updateDto.FirstName))
//        person.FirstName = updateDto.FirstName;

//    if (!string.IsNullOrEmpty(updateDto.LastName))
//        person.LastName = updateDto.LastName;

//    if (!string.IsNullOrEmpty(updateDto.PersonalNumber))
//        person.PersonalNumber = updateDto.PersonalNumber;

//    person.Gender = updateDto.Gender ?? person.Gender;
//    person.BirthDay = updateDto.BirthDay ?? person.BirthDay;
//    person.City = updateDto.City ?? person.City;

//    var phoneNumber = await dbContext.PhoneNumber.FirstOrDefaultAsync(p => p.PersonId == person.Id);

//    if (phoneNumber != null && updateDto.PhoneNumbers != null)
//    {
//        phoneNumber.MobileNumber = !string.IsNullOrEmpty(updateDto.PhoneNumbers.MobileNumber) ? updateDto.PhoneNumbers.MobileNumber : phoneNumber.MobileNumber;
//        phoneNumber.HomeNumber = !string.IsNullOrEmpty(updateDto.PhoneNumbers.HomeNumber) ? updateDto.PhoneNumbers.HomeNumber : phoneNumber.HomeNumber;
//        phoneNumber.OfficeNumber = !string.IsNullOrEmpty(updateDto.PhoneNumbers.OfficeNumber) ? updateDto.PhoneNumbers.OfficeNumber : phoneNumber.OfficeNumber;
//    }

//    await dbContext.SaveChangesAsync();
//    return Ok(person);
//}