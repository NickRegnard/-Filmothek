using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class MovieListData
    {

        public List<Movie> movies { get; set; }
        public int TotalHits { get; set; }

        public MovieListData(List<Movie> movieList, int totalHits)
        {
            movies = movieList;
            TotalHits = totalHits;
        }
    //    public int Id { get; set; }
    //    public string MovieName { get; set; }
    //    public string Genre { get; set; }
    //    public int? Length { get; set; }
    //    public bool IsSeries { get; set; }
    //    public float? Rating { get; set; }
    //    public float? Price { get; set; }
    //    public string LanguageDub { get; set; }
    //    public string LanguageSub { get; set; }
    //    public DateTime Release { get; set; }
    //    public int? FSK { get; set; }
    //    public string Content { get; set; }

    //    public MovieListData(Movie movie)
    //    {
    //        Id = movie.Id;
    //        MovieName = movie.MovieName;
    //        Genre = movie.Genre;
    //        Length = movie.Length;
    //        IsSeries = movie.IsSeries;
    //        Rating = movie.Rating;
    //        Price = movie.Price;
    //        LanguageDub = movie.LanguageDub;
    //        LanguageSub = movie.LanguageSub;
    //        Release = movie.Release;
    //        FSK = movie.FSK;
    //        Content = movie.Content;

    //    }
     }
}
