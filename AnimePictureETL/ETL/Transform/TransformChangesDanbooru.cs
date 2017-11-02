using AnimePictureETL.ExtractedData;
using AnimePictureETL.ExtractedData.Danbooru;
using AnimePictureETL.Models;
using AnimePictureETL.Repositories;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using AnimePictureETL.Extensions;

namespace AnimePictureETL.ETL.Transform
{
    public class TransformChangesDanbooru
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDictionary<string, string[]> _possibleTransformations;

        private DanbooruChangedObjects _changedObjects;

        public TransformChangesDanbooru(IDictionary<string, string[]> possibleTransformations)
        {
            _possibleTransformations = possibleTransformations;
            _changedObjects = new DanbooruChangedObjects();
        }

        public DanbooruChangedObjects Transform(IList<Post> changes)
        {
            foreach (var change in changes)
            {
                if (!change.Exists)
                {
                    log.Info("Transform: starting transforming picture of ID: " + change.DanbooruId.ToString());
                    Picture newPicture = TransformPicture(change);
                    TransformArtists(change, newPicture);
                    TransformCharacters(change, newPicture);
                    TransformTags(change, newPicture);
                }
            }
            return _changedObjects;
        }

        private Picture TransformPicture(Post change)
        {
            Picture picture = new Picture
            {
                Artists = new HashSet<Artist>(),
                Characters = new HashSet<Character>(),
                CharacterTags = new HashSet<CharacterTag>()
            };
            picture.Sources = new Source { Picture = picture, DanbooruId = change.DanbooruId };
            picture.Size = (int)change.Size;
            picture.Width = change.Width;
            picture.Height = change.Height;
            picture.Source = change.Source;
            picture.Md5Checksum = change.Checksum;
            picture.FileName = string.Format("/Images/{0}", change.FileName);
            picture.UploadDate = DateTime.Now;
            _changedObjects.ChangedPictures.Add(picture);
            return picture;
        }

        private void TransformArtists(Post change, Picture picture)
        {
            if (change.Artists.Length == 0)
                return;
            using (ArtistRepository artistRepo = new ArtistRepository())
            {
                foreach (var artistName in change.Artists)
                {
                    string newArtistName = artistName.ToLower();
                    newArtistName = newArtistName.Replace(' ', '_');
                    newArtistName = newArtistName.Capitalize();
                    Artist artistObj = artistRepo.GetByName(newArtistName);
                    if (artistObj == null)
                    {
                        var artistSet = _changedObjects.ChangedArtists.Where(a => a.Name == newArtistName);
                        bool extractedAlready = artistSet.Count() != 0;
                        if (!extractedAlready)
                        {
                            artistObj = new Artist
                            {
                                Name = newArtistName,
                                Pictures = new HashSet<Picture>()
                            };
                        }
                        else
                        {
                            artistObj = artistSet.Take(1).SingleOrDefault();
                        }
                    }
                    artistObj.Pictures.Add(picture);
                    picture.Artists.Add(artistObj);
                    _changedObjects.ChangedArtists.Add(artistObj);
                }
            }
        }

        private void TransformCharacters(Post change, Picture picture)
        {
            if (change.Characters.Length == 0)
                return;
            using (CharacterRepository charRepo = new CharacterRepository())
            {
                foreach (var characterName in change.Characters)
                {
                    Character character = charRepo.GetByDanbooruName(characterName);
                    if (character == null)
                    {
                        bool extractedAlready = false;
                        IEnumerable<Character> characterSet;
                        char[] sep = { ' ' };
                        string[] characterTrueName = characterName.Split(sep, 2);
                        for (int i = 0; i < characterTrueName.Length; i++)
                        {
                            characterTrueName[i] = characterTrueName[i].Capitalize();
                        }
                        if (characterTrueName.Length == 1)
                            characterSet = _changedObjects.ChangedCharacters.Where(c => c.Name == characterTrueName[0]);
                        else
                            characterSet = _changedObjects.ChangedCharacters.Where(c => c.Name == characterTrueName[1] && c.FamilyName == characterTrueName[0]);
                        extractedAlready = characterSet.Count() != 0;
                        if (!extractedAlready)
                        {
                            character = new Character
                            {
                                Pictures = new HashSet<Picture>(),
                                CharacterTags = new HashSet<CharacterTag>()
                            };
                            if (characterTrueName.Length != 1)
                            {
                                character.Name = characterTrueName[1];
                                character.FamilyName = characterTrueName[0];
                            }
                            else
                                character.Name = characterTrueName[0];
                        }
                        else
                        {
                            character = characterSet.Single();
                        }
                        if (!_changedObjects.ChangedCharacters.Contains(character))
                            _changedObjects.ChangedCharacters.Add(character);
                    }
                    character.Pictures.Add(picture);
                    picture.Characters.Add(character);
                }
            }
        }

        private void TransformTags(Post change, Picture picture)
        {
            if (change.Tags.Length == 0)
                return;
            using (CharacterTagRepository charRepo = new CharacterTagRepository())
            {
                Tag tagObj = null;
                foreach (var tag in change.Tags)
                {
                    string newTag = tag.ToLower().Replace(' ', '_').Capitalize();

                    CharacterTag chTag = new CharacterTag();

                    picture.CharacterTags.Add(chTag);
                    chTag.Picture = picture;

                    var character = picture.Characters.First();
                    chTag.Character = character;
                    character.CharacterTags.Add(chTag);

                    tagObj = charRepo.GetTagByName(newTag);
                    if (tagObj == null)
                    {
                        tagObj = new Tag
                        {
                            Name = newTag,
                            CharacterTags = new HashSet<CharacterTag>()
                        };
                    }
                    tagObj.CharacterTags.Add(chTag);
                    chTag.Tag = tagObj;
                    _changedObjects.ChangedTags.Add(chTag);
                }
            }
        }
        
    }
}