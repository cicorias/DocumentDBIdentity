using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DX.TED.DocumentDb.Identity.Tests
{
    internal static class Helpers
    {
        internal static void DeleteDatabase(this DocumentDbContext context)
        {
            try
            {
                DocumentClient client = new DocumentClient(context.Uri, context.AuthKey);

                client.DeleteDatabaseAsync(context.Database.SelfLink).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var msgEx = ex as AggregateException;
                if (null != msgEx)
                {
                    foreach (var item in msgEx.Flatten().InnerExceptions)
                    {
                        Console.WriteLine(item.Message);
                    }
                }

            }
        }

        internal static void DeleteUser(DocumentDbContext context, string userName)
        {
            Task<DocumentCollection> task1 = GetDocCollection(context);

            var idsToDelete = context.Client.CreateDocumentQuery<IdentityUser>(task1.Result.SelfLink)
                     .Where(f => f.UserName == userName).Select(c => c.Id).ToArray();

            Console.WriteLine("will be deleting {0} documents", idsToDelete.Length);
            //delete them all...
            foreach (var item in idsToDelete)
            {
                var result = context.Client.CreateDocumentQuery<Document>(task1.Result.SelfLink)
                         .Where(f => f.Id == item).ToArray().FirstOrDefault();

                if (null != result)
                {
                    Console.WriteLine("Deleting ID: {0}", result.Id);
                    context.Client.DeleteDocumentAsync(result.SelfLink).Wait();
                }
            }

        }

        internal static Task<DocumentCollection> GetDocCollection(DocumentDbContext context)
        {
            Task<DocumentCollection> task1 = GetOrCreateCollectionAsync(context.Client, context.Database.SelfLink, context.CollectionName);
            task1.Wait();
            return task1;
        }

        /// <summary>
        /// Get a DocuemntCollection by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="id">The id of the DocumentCollection to search for, or create.</param>
        /// <returns>The matched, or created, DocumentCollection object</returns>
        static async Task<DocumentCollection> GetOrCreateCollectionAsync(DocumentClient client, string dbLink, string id)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
            }

            return collection;
        }

        internal static bool CheckAggregateException(this AggregateException exception, string message)
        {
            var flat = exception.Flatten();
            foreach (var item in flat.InnerExceptions)
            {
                if (item.Message.ToLowerInvariant().Contains(message.ToLowerInvariant()))
                {
                    return true;
                }
            }

            return false;
        }


    }

    public class RandomString
    {
        const int s_length = 15;
        const string s_validCharacters = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$^()[]";

        public static string Generate()
        {
            return Generate(s_length);
        }

        public static string Generate(int length)
        {
            return Generate(length, s_validCharacters);
        }

        public static string Generate(int length, string validCharacters)
        {
            //string charsToUse = ;
            char[] chars;

            if (string.IsNullOrEmpty(validCharacters))
                chars = s_validCharacters.ToCharArray();
            else
                chars = validCharacters.ToCharArray();

            int size = length;

            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = length;

            data = new byte[size];
            crypto.GetNonZeroBytes(data);

            StringBuilder result = new StringBuilder(size);

            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }

            return result.ToString();
        }




    }
}
