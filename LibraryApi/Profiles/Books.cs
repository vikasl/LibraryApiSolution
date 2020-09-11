﻿using AutoMapper;
using LibraryApi.Domain;
using LibraryApi.Models.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Profiles
{
    public class Books:Profile
    {
        public Books()
        {
            CreateMap<Book, GetBooksResponseItem>();
            CreateMap<Book, GetBookDetailsResponse>();
            CreateMap<BookCreateRequest, Book>()
                    .ForMember(dest => dest.RemovedFromInventory, d => d.MapFrom(_=>false));
        }
    }
}
