using ImageGallery.API.Entities;
using System;
using System.Collections.Generic;

namespace ImageGallery.API.Services
{
    public interface IGalleryRepository
    {
        bool IsImageOwner(string ownerId, Guid id);
        IEnumerable<Image> GetImages(string ownerId);
        Image GetImage(Guid id);
        bool ImageExists(Guid id);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(Image image);
        bool Save();
    }
}
