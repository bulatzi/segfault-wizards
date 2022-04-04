using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//! The Models namespace
/*!
 * This namespace falls under the AbetAPI namespace, and is for Models.
 * The Models are generally utilized to structure underlying data for the database, and utilized by EFModels
 * to provide composite functionality.
 */
namespace AbetApi.Models
{
    //! The UserWithRoles Class
    /*!
     * This class gets called by the UsersController
     */
    public class UserWithRoles
    {
        //! The user setter/getter function
        /*! 
         * This is a single line dual function for setting and getting
         */
        public AbetApi.EFModels.User user { get; set; }
        //! The roles setter/getter function
        /*! 
         * This is a single line dual function for setting and getting
         */
        public List<string> roles { get; set; }
    }
}
