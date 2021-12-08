using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace me.ewerestr.ewxtelegrambot.Utils
{
    public class EWXLocalData
    {
        public List<string> _postedImageList { get; set; } = new List<string>();
        public List<string> _unpostedImageList { get; set; } = new List<string>();

        public List<string> _postedAudioList { get; set; } = new List<string>();
        public List<string> _unpostedAudioList { get; set; } = new List<string>();

        [JsonIgnore]
        private Random random = new Random();

        public List<string> GetFullImageList()
        {
            return _postedImageList.Concat(_unpostedImageList).ToList();
        }

        public List<string> GetPostedImageList()
        {
            return _postedImageList;
        }

        public List<string> GetUnpostedImageList()
        {
            return _unpostedImageList;
        }

        public List<string> GetFullAudioList()
        {
            return _postedAudioList.Concat(_unpostedAudioList).ToList();
        }

        public List<string> GetPostedAudioList()
        {
            return _postedAudioList;
        }

        public List<string> GetUnpostedAudioList()
        {
            return _unpostedAudioList;
        }

        public bool HasImage(string image)
        {
            if (_postedImageList.Contains(image) || _unpostedImageList.Contains(image)) return true;
            return false;
        }

        public bool HasAudio(string audio)
        {
            if (_postedAudioList.Contains(audio) || _unpostedAudioList.Contains(audio)) return true;
            return false;
        }

        public bool HasImages()
        {
            return (_postedImageList.Count + _unpostedImageList.Count) > 0 ? true : false;
        }

        public bool HasAudios()
        {
            return (_postedAudioList.Count + _unpostedAudioList.Count) > 0 ? true : false;
        }

        public bool HasUnpostedImages()
        {
            return _unpostedImageList.Count > 0 ? true : false;
        }

        public bool HasUnpostedAudios()
        {
            return _unpostedAudioList.Count > 0 ? true : false;
        }
        
        public string GetRandomImage()
        {
            int index = random.Next(0, _unpostedImageList.Count - 1);
            _postedImageList.Add(_unpostedImageList[index]);
            _unpostedImageList.RemoveAt(index);
            return _postedImageList[_postedImageList.Count - 1];
        }

        public string GetRandomAudio()
        {
            int index = random.Next(0, _unpostedAudioList.Count - 1);
            _postedAudioList.Add(_unpostedAudioList[index]);
            _unpostedAudioList.RemoveAt(index);
            return _postedAudioList[_postedAudioList.Count - 1];
        }

        public void InitImages(string[] images)
        {
            _unpostedImageList = new List<string>(images);
        }

        public void InitAudios(string[] audios)
        {
            _unpostedAudioList = new List<string>(audios);
        }

        public void AddImage(string image)
        {
            _unpostedImageList.Add(image);
        }

        public void AddAudio(string audio)
        {
            _unpostedAudioList.Add(audio);
        }

        public void AddImages(List<string> images)
        {
            _unpostedImageList = _unpostedImageList.Concat(images).ToList();
        }

        public void AddAudios(List<string> audios)
        {
            _unpostedAudioList = _unpostedAudioList.Concat(audios).ToList();
        }

        public void RenewImages()
        {
            _unpostedImageList = new List<string>(_postedImageList);
            _postedImageList.Clear();
        }

        public void RenewAudios()
        {
            _unpostedAudioList = new List<string>(_postedAudioList);
            _postedAudioList.Clear();
        }

        public int GetImagesCount()
        {
            return _postedImageList.Count + _unpostedImageList.Count;
        }

        public int GetAudiosCount()
        {
            return _postedAudioList.Count + _unpostedAudioList.Count;
        }

        public void Save()
        {
            EWXTelegramBot.SaveLocalData(this);
        }
    }
}
