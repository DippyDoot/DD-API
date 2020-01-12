using System;
using System.Collections.Generic;
using System.Text;
using Dippy.DDApi.DomainModels;

namespace Dippy.DDApi {
    public interface IDDApi {
        //just a big todo at this point

        #region Collection
        bool AddGalleryToCollection(long collectionTagId, long galleryId, long? order = null);
        bool AddGalleriesToCollection(long collectionTagId, IEnumerable<(long galleryId, long? order)> galleries);
        void RemoveGalleryFromCollection(long collectionTagId, long galleryId);
        void RemoveGalleriesFromColection(long collectionTagId, IEnumerable<long> galleryIds);
        void RemoveCollection(long collectionId);
        IEnumerable<long> GetCollectionsWithGallery(long galleryId);
        IEnumerable<long> GetGalleriesInCollection(long collectionTagId);
        #endregion

        #region Favorite
        long FavoriteGallery(long galleryId);
        void UnfavoriteGallery(long galleryId);
        IEnumerable<long> PaginateFavoriteGalleriesByDate(int page, int pageSize, bool decendingOrder = true);

        long FavoriteTag(long tagId);
        void UnfavoriteTag(long tagId);
        IEnumerable<long> PaginateFavoriteTagsByDate(int page, int pageSize, bool decendingOrder = true);
        #endregion

        #region Gallery
        //need paginations, searches
        #endregion

        #region Alias
        IEnumerable<long> GetAliasesForGallery(long galleryId);

        IEnumerable<long> GetAliasesForTag(long tagId);
        #endregion

        #region Relation
        bool AddGalleryRelation(long fromGalleryId, long toGalleryId, bool twoWay = true);
        void RemoveGalleryRelation(long fromGalleryId, long toGalleryId, bool twoWay = true);

        IEnumerable<long> GetRelatedGalleries(long galleryId);

        long AddTagRelation(long fromTagId, long toTagId, bool twoWay = true);
        void RemoveTagRelation(long fromTagId, long toTagId, bool twoWay = true);

        IEnumerable<long> GetRelatedTags(long tagId);
        #endregion

        #region GalleryTag
        bool AddGalleryTag(GalleryTag galleryTag);
        bool AddGalleryTags(IEnumerable<GalleryTag> galleryTag);
        void RemoveGalleryTag(long galleryId, long tagId);
        void RemoveGalleryTag(GalleryTag galleryTag);
        void RemoveGalleryTags(IEnumerable<(long galleryId, long tagId)> galleryTags);
        void RemoveGalleryTags(IEnumerable<GalleryTag> galleryTags);
        void ChangeGalleryTagGroup(GalleryTag galleryTag);

        IEnumerable<GalleryTag> GetTags(long galleryId);
        IEnumerable<long> GetGalleriesWithTag(long tagId, int page, int pageSize, bool decendingOrder = true); //need args for order
        IEnumerable<long> GetGalleriesWithTagInGroup(long tagId, long groupId, int page, int pageSize, bool decendingOrder = true);
        #endregion

        #region GalleryType
        //long AddTagType(string typeName);
        //void RemoveTagType(long galleryTypeId);

        //TagType GetTagType(long typeId);
        //string GetNameOfTagType(long typeId);
        #endregion

        #region ResourceFile
        #endregion

        #region SourceGroup
        #endregion

        #region SourceInfo
        //need paginate and search
        #endregion

        #region Tag
        //need paginate and search
        #endregion

        #region TagGroup
        //need paginate and search
        #endregion

        #region TagType
        #endregion
    }
}
