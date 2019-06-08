using System;
using MediaManager.Logging;
using MediaManager.SABnzbd.JsonObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MediaManager.SABnzbd.SabCommands
{
	public partial class SabManager
	{
		public class SetPaused : SabClientCommand
		{
			public override string CommandText()
			{
				return "&mode=pause";
			}
		}

		public class SetUnPaused : SabClientCommand
		{
			public override string CommandText()
			{
				return "&mode=resume";
			}
		}

		public class SetDeleteJob : SabClientCommand
		{
			private string id = "";

			public SetDeleteJob(string nzoId)
			{
				id = nzoId;
			}

			public override string CommandText()
			{
				return $"&mode=queue&name=delete&value={id}";
			}
		}

		public class SetForceJob : SabClientCommand
		{
			private string id = "";

			public SetForceJob(string nzoId)
			{
				id = nzoId;
			}

			public override string CommandText()
			{
				return $"&mode=queue&name=priority&value={id}&value2=2";
			}
		}

		public class GetQueue : SabClientCommand
		{
			public override string CommandText()
			{
				return "&mode=queue";
			}

			public override IJsonBase Parse(string rawJSON)
			{
				//rawJSON = "{\"queue\": {\"noofslots_total\": 31, \"diskspace2_norm\": \"3.2 T\", \"paused\": true, \"finish\": 0, \"speedlimit_abs\": \"\", \"slots\": [{\"status\": \"Queued\", \"index\": 0, \"password\": \"\", \"missing\": 0, \"avg_age\": \"23d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"241.33\", \"sizeleft\": \"237 MB\", \"filename\": \"Robot.Chicken.S09E07.720p.HDTV.x264-W4F-xpost\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"237.10\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"1\", \"nzo_id\": \"SABnzbd_nzo_wkpmnb\", \"unpackopts\": \"3\", \"size\": \"241 MB\"}, {\"status\": \"Queued\", \"index\": 1, \"password\": \"\", \"missing\": 0, \"avg_age\": \"22d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"275.64\", \"sizeleft\": \"276 MB\", \"filename\": \"The.Daily.Show.2018.02.05.Jessica.Williams.WEB.x264-TBS[rarbg]-xpost\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"275.64\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_0emyvy\", \"unpackopts\": \"3\", \"size\": \"276 MB\"}, {\"status\": \"Queued\", \"index\": 2, \"password\": \"\", \"missing\": 0, \"avg_age\": \"21d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"2838.50\", \"sizeleft\": \"2.8 GB\", \"filename\": \"The.Late.Show.with.Stephen.Colbert.S03E85.Claire.Danes.Bernadette.Peters.Lil.Uzi.Vert.1080p.AMZN.WEB-DL.DDP2.0.H.264-monkee\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"2838.50\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_f6ts5d\", \"unpackopts\": \"3\", \"size\": \"2.8 GB\"}, {\"status\": \"Queued\", \"index\": 3, \"password\": \"\", \"missing\": 0, \"avg_age\": \"21d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"255.48\", \"sizeleft\": \"255 MB\", \"filename\": \"The.Daily.Show.2018.02.06.Liz.Claman.WEB.x264-TBS-xpost\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"255.48\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_panmna\", \"unpackopts\": \"3\", \"size\": \"255 MB\"}, {\"status\": \"Queued\", \"index\": 4, \"password\": \"\", \"missing\": 0, \"avg_age\": \"18d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"2056.18\", \"sizeleft\": \"2.0 GB\", \"filename\": \"Real.Time.With.Bill.Maher.S16E04.720p.WEB-DL.AAC2.0.H.264-doosh\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"2056.18\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_pn2dir\", \"unpackopts\": \"3\", \"size\": \"2.0 GB\"}, {\"status\": \"Queued\", \"index\": 5, \"password\": \"\", \"missing\": 0, \"avg_age\": \"11d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"3476.96\", \"sizeleft\": \"3.4 GB\", \"filename\": \"Real.Time.with.Bill.Maher.S16E05.1080p.AMZN.WEB-DL\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"3476.96\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_ogmhgz\", \"unpackopts\": \"3\", \"size\": \"3.4 GB\"}, {\"status\": \"Queued\", \"index\": 6, \"password\": \"\", \"missing\": 0, \"avg_age\": \"9d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"647.17\", \"sizeleft\": \"647 MB\", \"filename\": \"Last.Week.Tonight.with.John.Oliver.S05E01.720p.AMZN.WEB-DL.DDP2.0.H.264-monkee\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"647.17\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_mtohw4\", \"unpackopts\": \"3\", \"size\": \"647 MB\"}, {\"status\": \"Queued\", \"index\": 7, \"password\": \"\", \"missing\": 0, \"avg_age\": \"11d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"3476.96\", \"sizeleft\": \"3.4 GB\", \"filename\": \"Real.Time.with.Bill.Maher.S16E05.1080p.AMZN.WEB-DL\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"3476.96\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_oofuqr\", \"unpackopts\": \"3\", \"size\": \"3.4 GB\"}, {\"status\": \"Queued\", \"index\": 8, \"password\": \"\", \"missing\": 0, \"avg_age\": \"8d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"353.99\", \"sizeleft\": \"354 MB\", \"filename\": \"Real.Time.with.Bill.Maher.S16E05.Overtime.1080p.AMZN.WEB-DL.DDP2.0.H.264-monkee\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"353.99\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_hgcsza\", \"unpackopts\": \"3\", \"size\": \"354 MB\"}, {\"status\": \"Queued\", \"index\": 9, \"password\": \"\", \"missing\": 0, \"avg_age\": \"9d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"1740.06\", \"sizeleft\": \"1.7 GB\", \"filename\": \"Last.Week.Tonight.with.John.Oliver.S05E01.1080p.AMZN.WEB-DL.DDP2.0.H.264-monkee\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"1740.06\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_9roydr\", \"unpackopts\": \"3\", \"size\": \"1.7 GB\"}, {\"status\": \"Queued\", \"index\": 10, \"password\": \"\", \"missing\": 0, \"avg_age\": \"8d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"353.99\", \"sizeleft\": \"354 MB\", \"filename\": \"Real.Time.with.Bill.Maher.S16E05.Overtime.1080p.AMZN.WEB-DL.DDP2.0.H.264-monkee\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"353.99\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_taidw2\", \"unpackopts\": \"3\", \"size\": \"354 MB\"}, {\"status\": \"Queued\", \"index\": 11, \"password\": \"\", \"missing\": 0, \"avg_age\": \"9d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"1740.06\", \"sizeleft\": \"1.7 GB\", \"filename\": \"Last.Week.Tonight.with.John.Oliver.S05E01.1080p.AMZN.WEB-DL.DDP2.0.H.264-monkee\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"1740.06\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_rf_kw4\", \"unpackopts\": \"3\", \"size\": \"1.7 GB\"}, {\"status\": \"Queued\", \"index\": 12, \"password\": \"\", \"missing\": 0, \"avg_age\": \"1d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"1944.91\", \"sizeleft\": \"1.9 GB\", \"filename\": \"Last.Week.Tonight.with.John.Oliver.S05E02.1080p.AMZN.WEB-DL.DDP2.0.H.264-monkee\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"1944.91\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_kuxsm3\", \"unpackopts\": \"3\", \"size\": \"1.9 GB\"}, {\"status\": \"Queued\", \"index\": 13, \"password\": \"\", \"missing\": 0, \"avg_age\": \"4d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"1985.96\", \"sizeleft\": \"1.9 GB\", \"filename\": \"Real.Time.With.Bill.Maher.S16E06.720p.WEB-DL.AAC2.0.H.264-doosh\", \"priority\": \"High\", \"cat\": \"sonarr\", \"mbleft\": \"1985.96\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_plrq4w\", \"unpackopts\": \"3\", \"size\": \"1.9 GB\"}, {\"status\": \"Queued\", \"index\": 14, \"password\": \"\", \"missing\": 0, \"avg_age\": \"418d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"4348.98\", \"sizeleft\": \"4.2 GB\", \"filename\": \"The.Girl.With.All.The.Gifts.2016.720p.HDRip.X264.AC3-EVO.cp(tt4547056)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"4348.98\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_2ypdu8\", \"unpackopts\": \"3\", \"size\": \"4.2 GB\"}, {\"status\": \"Queued\", \"index\": 15, \"password\": \"\", \"missing\": 0, \"avg_age\": \"273d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"1631.60\", \"sizeleft\": \"1.6 GB\", \"filename\": \"the lego batman movie 2017 brrip xvid ac3-evo avi.cp(tt4116284)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"1631.60\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_hgfzpd\", \"unpackopts\": \"3\", \"size\": \"1.6 GB\"}, {\"status\": \"Queued\", \"index\": 16, \"password\": \"\", \"missing\": 0, \"avg_age\": \"265d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"12102.67\", \"sizeleft\": \"11.8 GB\", \"filename\": \"John.Wick.Chapter.2.2017.INTERNAL.1080p.BluRay.CRF.x264-SAPHiRE.cp(tt4425200)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"12102.67\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_er6zn3\", \"unpackopts\": \"3\", \"size\": \"11.8 GB\"}, {\"status\": \"Queued\", \"index\": 17, \"password\": \"\", \"missing\": 0, \"avg_age\": \"184d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"6234.47\", \"sizeleft\": \"6.1 GB\", \"filename\": \"Captain.Underpants.The.First.Epic.Movie.2017.MULTi.1080p.BluRay.x264.cp(tt2091256)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"6234.47\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_o2xelr\", \"unpackopts\": \"3\", \"size\": \"6.1 GB\"}, {\"status\": \"Queued\", \"index\": 18, \"password\": \"\", \"missing\": 0, \"avg_age\": \"273d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"967.12\", \"sizeleft\": \"967 MB\", \"filename\": \"The.Exception.2016.WEB-DL.x264-FGT.cp(tt4721124)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"967.12\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_jykou5\", \"unpackopts\": \"3\", \"size\": \"967 MB\"}, {\"status\": \"Queued\", \"index\": 19, \"password\": \"\", \"missing\": 0, \"avg_age\": \"82d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"18328.01\", \"sizeleft\": \"17.9 GB\", \"filename\": \"Kingsman.The.Golden.Circle.2017.1080p.BluRay.DTS.x264-FuzerHD.cp(tt4649466)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"18328.01\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_kvinlg\", \"unpackopts\": \"3\", \"size\": \"17.9 GB\"}, {\"status\": \"Queued\", \"index\": 20, \"password\": \"\", \"missing\": 0, \"avg_age\": \"84d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"726.89\", \"sizeleft\": \"727 MB\", \"filename\": \"The.Villainess.2017.BDRip.x264-NODLABS.cp(tt6777338)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"726.89\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_1eg7nj\", \"unpackopts\": \"3\", \"size\": \"727 MB\"}, {\"status\": \"Queued\", \"index\": 21, \"password\": \"\", \"missing\": 0, \"avg_age\": \"176d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"4895.39\", \"sizeleft\": \"4.8 GB\", \"filename\": \"The.Big.Sick.2017.1080p.WEB-DL.DD5.1.H264.cp(tt5462602)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"4895.39\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_wcnucf\", \"unpackopts\": \"3\", \"size\": \"4.8 GB\"}, {\"status\": \"Queued\", \"index\": 22, \"password\": \"\", \"missing\": 0, \"avg_age\": \"217d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"1642.85\", \"sizeleft\": \"1.6 GB\", \"filename\": \"Wakefield.2017.BRRip.XviD.AC3-EVO.cp(tt5195412)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"1642.85\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_h2yx7u\", \"unpackopts\": \"3\", \"size\": \"1.6 GB\"}, {\"status\": \"Queued\", \"index\": 23, \"password\": \"\", \"missing\": 0, \"avg_age\": \"129d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"8900.77\", \"sizeleft\": \"8.7 GB\", \"filename\": \"Bushwick.2017.1080p.BluRay.x264-EiDER.cp(tt4720702)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"8900.77\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_7thovn\", \"unpackopts\": \"3\", \"size\": \"8.7 GB\"}, {\"status\": \"Queued\", \"index\": 24, \"password\": \"\", \"missing\": 0, \"avg_age\": \"38d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"8772.21\", \"sizeleft\": \"8.6 GB\", \"filename\": \"Blade.Runner.2049.2017.PROPER.720p.BluRay.x264-BLOW.cp(tt1856101)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"8772.21\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_zckra_\", \"unpackopts\": \"3\", \"size\": \"8.6 GB\"}, {\"status\": \"Queued\", \"index\": 25, \"password\": \"\", \"missing\": 0, \"avg_age\": \"188d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"4683.62\", \"sizeleft\": \"4.6 GB\", \"filename\": \"Megan Leavey - 2017 1080p WEB-DL.cp(tt4899370)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"4683.62\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_ratn6r\", \"unpackopts\": \"3\", \"size\": \"4.6 GB\"}, {\"status\": \"Queued\", \"index\": 26, \"password\": \"\", \"missing\": 0, \"avg_age\": \"893d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"9812.03\", \"sizeleft\": \"9.6 GB\", \"filename\": \"Blade.Runner.1982.The.Final.Cut.720p.BluRay.DTS.AC3.x264-CyTSuNee.cp(tt0083658)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"9812.03\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_0tkfjk\", \"unpackopts\": \"3\", \"size\": \"9.6 GB\"}, {\"status\": \"Queued\", \"index\": 27, \"password\": \"\", \"missing\": 0, \"avg_age\": \"1311d\", \"script\": \"None\", \"has_rating\": true, \"mb\": \"15005.31\", \"sizeleft\": \"14.7 GB\", \"rating_avg_video\": 0, \"filename\": \"Chinese Zodiac 1080p 2013 tt1424310.cp(tt1446714)\", \"priority\": \"Normal\", \"rating_avg_audio\": 0, \"cat\": \"couchpotato\", \"mbleft\": \"15005.31\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_pfpszo\", \"unpackopts\": \"3\", \"size\": \"14.7 GB\"}, {\"status\": \"Queued\", \"index\": 28, \"password\": \"\", \"missing\": 0, \"avg_age\": \"288d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"5152.84\", \"sizeleft\": \"5.0 GB\", \"filename\": \"LOGAN (2017) x264 1080p (WEB-DL) DD5 1 NLSubs -QoQ.cp(tt3315342)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"5152.84\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_bhuoj5\", \"unpackopts\": \"3\", \"size\": \"5.0 GB\"}, {\"status\": \"Queued\", \"index\": 29, \"password\": \"\", \"missing\": 0, \"avg_age\": \"12d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"5795.83\", \"sizeleft\": \"5.7 GB\", \"filename\": \"Call Me By Your Name 2017 1080p WEB-DL H264 AC3-EVO mkv.cp(tt5726616)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"5795.83\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_ithnsa\", \"unpackopts\": \"3\", \"size\": \"5.7 GB\"}, {\"status\": \"Queued\", \"index\": 30, \"password\": \"\", \"missing\": 0, \"avg_age\": \"9d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"1021.87\", \"sizeleft\": \"1022 MB\", \"filename\": \"The Death of Stalin 2017 720p WEB-DL.cp(tt4686844)\", \"priority\": \"Normal\", \"cat\": \"couchpotato\", \"mbleft\": \"1021.87\", \"eta\": \"unknown\", \"timeleft\": \"0:00:00\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_xxu23w\", \"unpackopts\": \"3\", \"size\": \"1022 MB\"}], \"speed\": \"0  \", \"size\": \"128.3 GB\", \"rating_enable\": true, \"eta\": \"unknown\", \"refresh_rate\": \"2\", \"start\": 0, \"version\": \"2.0.1\", \"diskspacetotal2\": \"5588.90\", \"limit\": 0, \"diskspacetotal1\": \"5588.90\", \"status\": \"Paused\", \"have_warnings\": \"0\", \"cache_art\": \"0\", \"sizeleft\": \"128.3 GB\", \"finishaction\": null, \"paused_all\": false, \"quota\": \"0 \", \"have_quota\": false, \"mbleft\": \"131405.41\", \"diskspace2\": \"3261.44\", \"diskspace1\": \"3261.44\", \"scripts\": [], \"categories\": [\"*\", \"couchpotato\", \"sonarr\"], \"timeleft\": \"0:00:00\", \"pause_int\": \"0\", \"noofslots\": 31, \"mb\": \"131409.64\", \"loadavg\": \"\", \"cache_max\": \"471859200\", \"kbpersec\": \"0.00\", \"speedlimit\": \"100\", \"cache_size\": \"0 B\", \"left_quota\": \"0 \", \"diskspace1_norm\": \"3.2 T\", \"queue_details\": \"0\"}}";

				//rawJSON = "{ \"queue\": { \"noofslots_total\": 4, \"diskspace2_norm\": \"3.2 T\", \"paused\": false, \"finish\": 0, \"speedlimit_abs\": \"\", \"slots\": [ { \"status\": \"Downloading\", \"index\": 0, \"password\": \"\", \"missing\": 358, \"avg_age\": \"1347d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"3130.50\", \"sizeleft\": \"2.8 GB\", \"filename\": \"Black.Sails.S01E01.720p.BluRay.x264-DEMAND\", \"priority\": \"Normal\", \"cat\": \"sonarr\", \"mbleft\": \"2849.72\", \"eta\": \"22:16 Thu 23 Aug\", \"timeleft\": \"81:00:41:17\", \"percentage\": \"8\", \"nzo_id\": \"SABnzbd_nzo_1zvuxq\", \"unpackopts\": \"3\", \"size\": \"3.1 GB\" }, { \"status\": \"Downloading\", \"index\": 1, \"password\": \"\", \"missing\": 0, \"avg_age\": \"1347d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"2614.10\", \"sizeleft\": \"2.6 GB\", \"filename\": \"Black.Sails.S01E02.720p.BluRay.x264-DEMAND\", \"priority\": \"Normal\", \"cat\": \"sonarr\", \"mbleft\": \"2614.10\", \"eta\": \"06:10 Tue 06 Nov\", \"timeleft\": \"155:08:35:11\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_jgi5qn\", \"unpackopts\": \"3\", \"size\": \"2.6 GB\" }, { \"status\": \"Downloading\", \"index\": 2, \"password\": \"\", \"missing\": 0, \"avg_age\": \"1347d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"2619.87\", \"sizeleft\": \"2.6 GB\", \"filename\": \"Black.Sails.S01E03.720p.BluRay.x264-DEMAND\", \"priority\": \"Normal\", \"cat\": \"sonarr\", \"mbleft\": \"2619.87\", \"eta\": \"18:00 Sat 19 Jan\", \"timeleft\": \"229:20:25:15\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_eilgxg\", \"unpackopts\": \"3\", \"size\": \"2.6 GB\" }, { \"status\": \"Downloading\", \"index\": 3, \"password\": \"\", \"missing\": 0, \"avg_age\": \"1345d\", \"script\": \"None\", \"has_rating\": false, \"mb\": \"2622.00\", \"sizeleft\": \"2.6 GB\", \"filename\": \"Black.Sails.S01E04.REPACK.720p.BluRay.x264-DEMAND\", \"priority\": \"Normal\", \"cat\": \"sonarr\", \"mbleft\": \"2622.00\", \"eta\": \"07:17 Thu 04 Apr\", \"timeleft\": \"304:09:42:31\", \"percentage\": \"0\", \"nzo_id\": \"SABnzbd_nzo_whmjpp\", \"unpackopts\": \"3\", \"size\": \"2.6 GB\" } ], \"speed\": \"427  \", \"size\": \"10.7 GB\", \"rating_enable\": true, \"eta\": \"07:17 Thu 04 Apr\", \"refresh_rate\": \"2\", \"start\": 0, \"version\": \"2.0.1\", \"diskspacetotal2\": \"5588.90\", \"limit\": \"0\", \"diskspacetotal1\": \"5588.90\", \"status\": \"Downloading\", \"have_warnings\": \"0\", \"cache_art\": \"0\", \"sizeleft\": \"10.5 GB\", \"finishaction\": null, \"paused_all\": false, \"quota\": \"0 \", \"have_quota\": false, \"mbleft\": \"10705.70\", \"diskspace2\": \"3273.78\", \"diskspace1\": \"3273.78\", \"scripts\": [], \"categories\": [ \"*\", \"couchpotato\", \"sonarr\" ], \"timeleft\": \"304:09:42:31\", \"pause_int\": \"0\", \"noofslots\": 4, \"mb\": \"10986.47\", \"loadavg\": \"\", \"cache_max\": \"471859200\", \"kbpersec\": \"0.42\", \"speedlimit\": \"100\", \"cache_size\": \"0 B\", \"left_quota\": \"0 \", \"diskspace1_norm\": \"3.2 T\", \"queue_details\": \"0\" } }";

				try
				{
					var value = JsonConvert.DeserializeObject<JsonQueue>(rawJSON, new JsonSerializerSettings
					{
						TraceWriter = TraceWriter,
						Converters = {
							new JavaScriptDateTimeConverter()
						}
					});

					//LogWriter.Write($"JSON # {rawJSON}", DebugPriority.Low);
					//LogWriter.Write($"JSON TRACE # {TraceWriter}", DebugPriority.Low);
					return value;
				}
				catch (Exception ex)
				{
					LogWriter.Write($"JSON EXCEPTION # {ex}");
					return null;
				}
			}
		}

		public class GetStatus : SabClientCommand
		{
			public override string CommandText()
			{
				return "&mode=fullstatus&skip_dashboard=0";
			}

			public override IJsonBase Parse(string rawJSON)
			{
				try
				{
					var value = JsonConvert.DeserializeObject<JsonStatus>(rawJSON, new JsonSerializerSettings
					{
						TraceWriter = TraceWriter,
						Converters = {
							new JavaScriptDateTimeConverter()
						}
					});

					//LogWriter.Write($"JSON # {rawJSON}", DebugPriority.Low);
					//LogWriter.Write($"JSON TRACE # {TraceWriter}", DebugPriority.Low);
					return value;
				}
				catch (Exception ex)
				{
					LogWriter.Write($"JSON EXCEPTION # {ex}");
					return null;
				}
			}
		}

		public class GetVersion : SabClientCommand
		{
			public override string CommandText()
			{
				return "&mode=version";
			}

			public override IJsonBase Parse(string rawJSON)
			{
				try
				{
					var value = JsonConvert.DeserializeObject<JsonVersion>(rawJSON, new JsonSerializerSettings
					{
						TraceWriter = TraceWriter,
						Converters = {
						new JavaScriptDateTimeConverter()
						}
					});

					LogWriter.Write($"JSON # {rawJSON}", DebugPriority.Low);
					LogWriter.Write($"JSON TRACE # {TraceWriter}", DebugPriority.Low);
					return value;
				}
				catch (Exception ex)
				{
					LogWriter.Write($"JSON EXCEPTION # {ex}");
					return null;
				}
			}
		}
	}
}