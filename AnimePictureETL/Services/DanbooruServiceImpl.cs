using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnimePictureETL.ExtractedData.Danbooru;
using AnimePictureETL.Extract.ETL;
using AnimePictureETL.ETL.Extract;
using AnimePictureETL.Extensions;
using System.Text;
using AnimePictureETL.Repositories;
using AnimePictureETL.Models;
using AnimePictureETL.ETL.Load;

namespace AnimePictureETL.Services
{
    public class DanbooruServiceImpl : IDanbooruService
    {
        private ExtractDanbooru extract = ExtractDanbooru.GetInstance();

        private LoadDanbooru load = LoadDanbooru.GetInstance();

        private RepositoryBase _repository;

        public DanbooruServiceImpl(RepositoryBase repository)
        {
            _repository = repository;
        }

        public Post ExtractPost(long id)
        {
            var post = extract.ExtractPost(id);
            return post;
        }

        public PostChangesView GetSuggestedValuesForPost(Post post)
        {
            var data = new PostChangesView
            {
                Height = post.Height,
                Width = post.Width,
                FileName = post.FileName,
                DanbooruId = post.DanbooruId
            };
            var characters = GetSuggestedCharacterData(post.Characters);
            data.Characters = characters;
            var artists = GetSuggestedArtistData(post.Artists);
            data.Artists = artists;
            var series = GetSuggestedSeriesData(post.Series);
            data.Series = series;
            var tags = GetSuggestedTagData(post.Tags);
            data.Tags = tags;
            data.Height = post.Height;
            data.Width = post.Width;
            data.Source = post.Source;
            data.FileName = post.FileName;
            data.Checksum = post.Checksum;
            data.Size = post.Size;
            return data;
        }

        private List<CharacterData> GetSuggestedCharacterData(string[] extractedCharacters)
        {
            var characters = new List<CharacterData>();
            foreach (var character in extractedCharacters)
            {
                var characterData = new CharacterData { Tag = character };
                bool exists = false;
                using (CharacterRepository repo = new CharacterRepository())
                {
                    exists = repo.DoExistsByDanbooruName(character);
                }
                characterData.Exists = exists;
                
                string suggestedName, suggestedFamilyName, nameValue, bracketValue;
                suggestedName = suggestedFamilyName = nameValue = bracketValue = string.Empty;

                if (!exists)
                {
                    int openBracketIndex = character.IndexOf('(');
                    if (openBracketIndex != -1)
                    {
                        int closeBracketIndex = character.IndexOf(')');
                        int length = closeBracketIndex - openBracketIndex - 1;
                        bracketValue = character.Substring(openBracketIndex + 1, length);
                        nameValue = character.Substring(0, openBracketIndex - 1);
                    }
                    else
                        nameValue = character;

                    if (nameValue.Contains(' '))
                    {
                        int firstSpaceIndex = nameValue.IndexOf(' ');
                        suggestedFamilyName = nameValue.Substring(0, firstSpaceIndex);
                        suggestedName = nameValue.Substring(firstSpaceIndex + 1);
                    }
                    else
                        suggestedName = nameValue;
                    suggestedName = suggestedName.Capitalize();
                    suggestedFamilyName = suggestedFamilyName.Capitalize();
                }
                else
                {
                    using (CharacterRepository repo = new CharacterRepository())
                    {
                        Character charObj = repo.GetByDanbooruName(character);
                        suggestedName = charObj.Name;
                        suggestedFamilyName = charObj.FamilyName;
                    }
                }

                characterData.SuggestedName = suggestedName;
                characterData.SuggestedFamilyName = suggestedFamilyName;
                characters.Add(characterData);
            }
            return characters;
        }

        private List<ArtistData> GetSuggestedArtistData(string[] extractedArtists)
        {
            var artists = new List<ArtistData>();
            foreach (var artist in extractedArtists)
            {
                var artistData = new ArtistData();
                artistData.Exists = false;
                artistData.Tag = artist;

                string suggestedName, suggestedNickname, nameValue, bracketValue;
                suggestedName = suggestedNickname = nameValue = bracketValue = string.Empty;

                int openBracketIndex = artist.IndexOf('(');
                if (openBracketIndex != -1)
                {
                    int closeBracketIndex = artist.IndexOf(')');
                    int length = closeBracketIndex - openBracketIndex - 1;
                    bracketValue = artist.Substring(openBracketIndex + 1, length);
                    nameValue = artist.Substring(0, openBracketIndex - 1);
                }
                else
                    nameValue = artist;
                
                if (!string.IsNullOrEmpty(bracketValue))
                    bracketValue = bracketValue.Capitalize();

                suggestedName = nameValue.Capitalize();
                suggestedNickname = bracketValue;
                artistData.SuggestedName = suggestedName;
                artistData.SuggestedNickname = suggestedNickname;
                artists.Add(artistData);
            }
            return artists;
        }

        private List<SeriesData> GetSuggestedSeriesData(string[] extractedSeries)
        {
            var series = new List<SeriesData>();
            foreach (var seriesTag in extractedSeries)
            {
                var seriesData = new SeriesData();
                seriesData.Exists = false;
                seriesData.Tag = seriesTag;

                string suggestedName = string.Empty;
                
                string[] nameParts = seriesTag.Split(' ');
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < nameParts.Length; ++i)
                {
                    sb.Append(nameParts[i].Capitalize());
                    if (i != nameParts.Length - 1)
                        sb.Append(' ');
                }
                suggestedName = sb.ToString();
                sb.Clear();
                nameParts = suggestedName.Split('/');
                for (int i = 0; i < nameParts.Length; ++i)
                {
                    sb.Append(nameParts[i].Capitalize());
                    if (i != nameParts.Length - 1)
                        sb.Append('/');
                }

                suggestedName = sb.ToString();
                seriesData.SuggestedName = suggestedName;
                series.Add(seriesData);
            }
            return series;
        }

        private List<TagData> GetSuggestedTagData(string[] extractedTags)
        {
            var tags = new List<TagData>();
            foreach (var tag in extractedTags)
            {
                var tagData = new TagData();
                tagData.Exists = false;
                tagData.Tag = tag;

                string suggestedTag = tag.Capitalize();
                tagData.SuggestedTag = suggestedTag;
                tags.Add(tagData);
            }
            return tags;
        }

        public void Save(PostChangesView data)
        {
            Picture picture = CreatePicture(data);
            CreateOrGetCharacters(picture, data.Characters);
            CreateOrGetArtists(picture, data.Artists);
            CreateOrGetSeries(picture, data.Series);
            CreateOrGetTags(picture, data.Tags);
            load.LoadPicture(picture);
        }

        private Picture CreatePicture(PostChangesView data)
        {
            Picture picture = new Picture
            {
                Characters = new HashSet<Character>(),
                Artists = new HashSet<Artist>(),
                Series = new HashSet<Series>(),
                CharacterTags = new HashSet<CharacterTag>(),
                Sources = new Source
                {
                    DanbooruId = data.DanbooruId
                },
                Width = data.Width,
                Height = data.Height,
                Source = data.Source,
                FileName = data.FileName,
                Md5Checksum = data.Checksum,
                Size = data.Size,
                UploadDate = DateTime.Now
            };
            return picture;
        }

        private void CreateOrGetCharacters(Picture picture, List<CharacterData> characters)
        {
            foreach (var characterData in characters)
            {
                Character character = null;
                if (!characterData.Exists)
                {
                    character = new Character
                    {
                        Name = characterData.SuggestedName,
                        FamilyName = characterData.SuggestedFamilyName,
                        DanbooruName = characterData.Tag,
                        Pictures = new HashSet<Picture>(),
                        CharacterTags = new HashSet<CharacterTag>()
                    };
                    character.Pictures.Add(picture);
                }
                else
                {
                    using (CharacterRepository repo = new CharacterRepository())
                    {
                        character = repo.GetByDanbooruName(characterData.Tag);
                        character.Pictures.Add(picture);
                    }
                }
                picture.Characters.Add(character);
            }
        }

        private void CreateOrGetArtists(Picture picture, List<ArtistData> artists)
        {
            foreach (var artistData in artists)
            {
                Artist artist = null;
                if (!artistData.Exists)
                {
                    artist = new Artist
                    {
                        Name = artistData.SuggestedName,
                        Pictures = new HashSet<Picture>()
                    };
                }
                else
                {
                    using (var repo = new ArtistRepository())
                    {
                        artist = repo.GetByName(artistData.SuggestedName);
                    }
                }
                artist.Pictures.Add(picture);
                picture.Artists.Add(artist);
            }
        }

        private void CreateOrGetSeries(Picture picture, List<SeriesData> series)
        {
            foreach (var seriesData in series)
            {
                Series seriesObj = null;
                if (!seriesData.Exists)
                {
                    seriesObj = new Series
                    {
                        Name = seriesData.SuggestedName,
                        Pictures = new HashSet<Picture>()
                    };
                }
                else
                {
                    using (var repo = new SeriesRepository())
                    {
                        seriesObj = repo.GetByName(seriesData.SuggestedName);
                    }
                }
                seriesObj.Pictures.Add(picture);
                picture.Series.Add(seriesObj);
            }
        }

        private void CreateOrGetTags(Picture picture, List<TagData> tags)
        {
            foreach (var tagData in tags)
            {
                if (tagData.CharacterTag == null)
                    continue;
                Tag tag = null;
                if (!tagData.Exists)
                {
                    tag = new Tag
                    {
                        CharacterTags = new HashSet<CharacterTag>(),
                        Name = tagData.SuggestedTag
                    };
                }
                else
                {
                    using (TagRepository repo = new TagRepository())
                    {
                        tag = repo.GetByName(tagData.SuggestedTag);
                    }
                }
                List<CharacterTag> charTags = new List<CharacterTag>();
                CharacterTag charTag = null;
                foreach (var character in tagData.CharacterTag)
                {
                    Character characterObj = picture.Characters.Where(c => c.DanbooruName == character).Take(1).SingleOrDefault();
                    charTag = new CharacterTag
                    {
                        Tag = tag,
                        Picture = picture,
                        Character = characterObj
                    };
                    tag.CharacterTags.Add(charTag);
                    picture.CharacterTags.Add(charTag);
                    characterObj.CharacterTags.Add(charTag);
                }
            }
        }
    }
}