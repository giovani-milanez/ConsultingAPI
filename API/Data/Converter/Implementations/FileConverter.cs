using API.Data.Converter.Contract;
using API.Data.VO;
using Database.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Data.Converter.Implementations
{
    public class FileConverter : IParser<File, FileDetailVO>
    {
        private string _baseUrl { get; set; }

        public FileConverter(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public FileDetailVO Parse(File origin)
        {
            if (origin == null) return null;
            var guid = new Guid(origin.Guid);
            return new FileDetailVO
            {
                Guid = guid,
                Name = origin.Name,
                Size = origin.Size,
                Type = origin.Type,
                Url = System.IO.Path.Combine(_baseUrl + "/" + guid)
        };
        }

        public List<FileDetailVO> Parse(List<File> origin)
        {
            if (origin == null) return null;
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
