using System;
using System.Collections.Generic;

namespace DertInfo.Models.Database
{
    public partial class Image : DatabaseEntity
    {
        public Image()
        {
            EventImages = new HashSet<EventImage>();
            GroupImages = new HashSet<GroupImage>();
            MarkingSheetImages = new HashSet<MarkingSheetImage>();
            TeamImages = new HashSet<TeamImage>();
        }

        public string Container { get; set; }
        public string BlobPath { get; set; }
        public string BlobName { get; set; }
        public string Extension { get; set; }
        public string ImageAlt { get; set; }

        /// <summary>
        /// Indicates that the image is to be removed from the storage account when we next perform a cleanup.
        /// </summary>
        public bool MarkedForRemoval { get; set; }

        /// <summary>
        /// This flag indicates that the image is protected from deletion as this is likely a default image.
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// This flag indicates that the image has been processed either as not for move or moved. 
        /// We use this to collect the images
        /// </summary>
        public bool HasBeenProcessedForMigration { get; set; }

        /// <summary>
        /// This flag indicates that the image has been moved to the new storage account.
        /// We use this to identify moved
        /// </summary>
        public bool HasBeenMovedForMigration { get; set; }

        [Obsolete("We have replaced this information when migrating all images to the new structure")]
        public string ImagePath { get; set; }
        [Obsolete("We have replaced this information when migrating all images to the new structure")]
        public string ImageUri { get; set; }
        
        public virtual ICollection<EventImage> EventImages { get; set; }
        public virtual ICollection<GroupImage> GroupImages { get; set; }
        public virtual ICollection<MarkingSheetImage> MarkingSheetImages { get; set; }
        public virtual ICollection<TeamImage> TeamImages { get; set; }
    }
}
