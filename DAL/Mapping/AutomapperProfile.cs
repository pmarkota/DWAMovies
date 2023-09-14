using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DAL.Mapping
{
    public class AutomapperProfile :Profile
    {
        public AutomapperProfile()
        {
            CreateMap<DALModels.Video, BLModels.BLVideo>().ReverseMap();
            CreateMap<DALModels.Genre, BLModels.BLGenre>().ReverseMap();
            CreateMap<DALModels.Tag, BLModels.BLTag>().ReverseMap();
            CreateMap<DALModels.Notification, BLModels.BLNotification>().ReverseMap();
            CreateMap<DALModels.User, BLModels.BLUser>().ReverseMap();
            CreateMap<DALModels.Image, BLModels.BLImage>().ReverseMap();
        }
    }
}
