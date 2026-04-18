import {
  _decorator, Component, Node, Label, director,
  RichText, UITransform, instantiate, Button, find,
  AudioSource, Animation, color,
} from "cc";

import { VP } from "../../../video-player/VP";
import { SoundPlay } from "../../App/Message/Events";
import { Postie } from "../../App/Share/Postie";
import { SoundManager } from "../../App/Share/SoundManager";
import { UrlConfig } from "../../Helper/UrlConfig";
import { Utill } from "../../Helper/Utill";
import { infos } from "../../info/game/info";
import { sound } from "../../info/game/sound";
import { Res_Form } from "../../Server/Res_Form";
import { WebAPI } from "../../Server/WebAPI/WebAPI";
import { LearningRecord } from "../../UI/Common/Learning/LearningRecord";
import { Record } from "../../UI/MiniGame01/Record";
import { Dubbing_Script } from "./Dubbing_Script";
import I18n from "../../Helper/i18n";

import { SplitDubbingController, ScoreWeight } from "./SplitDubbingController";
import { ScriptHighlightRenderer } from "./ScriptHighlightRenderer";

const { ccclass, property } = _decorator;

@ccclass("LearningDubbing")
export class LearningDubbing extends Component {

  // ── Inspector 노출 프로퍼티 ──────────────────────────────────────────────
  @property(Node) resultText: Node = null;
  @property(Node) starText: Node = null;
  @property(Node) onOffBtn: Node = null;
  @property(Node) onOffLabel: Node = null;
  @property(Node) plus: Node = null;
  @property(Node) minus: Node = null;
  @property(Node) dubbingStart: Node = null;
  @property(Node) dubbingStop: Node = null;
  @property(Node) scriptCon: Node = null;
  @property(Node) resultCon: Node = null;
  @property(Node) tryNumber: Node = null;
  @property(Label) paragraphLabel: Label = null;
  @property(Node) tryNumber2: Node = null;
  @property(Node) onAirSp: Node = null;
  @property(Node) listen: Node = null;
  @property(Node) countNode: Node = null;
  @property(Node) scriptNode: Node = null;
  @property(Node) dubbingText: Node = null;

  @property(Node) private dubbingModeBtn: Node = null;
  @property(Label) private dubbingModeText: Label = null;
  @property(RichText) private scriptHighlight: RichText = null;

  // ── 내부 상태 ────────────────────────────────────────────────────────────
  protected audioSource: AudioSource;
  private scene: string;
  private senData: any;               // TODO: 서버 응답 타입으로 교체
  private stars: string = " ";
  private starScript: boolean = false;
  private onOffScript: Label;
  private resultBtn: Node;
  private tryNum: Label;
  private tryNum2: Label;
  private video: VP;
  private record: LearningRecord;

  public count: number = 0;
  private isOnAir: boolean = false;
  isRecording: boolean = false;
  isComplete: boolean = false;

  private script: Dubbing_Script;
  private act_id: string;
  private gbn: string;
  private weight: ScoreWeight;

  words: { word: string; score: any }[] = [];
  excellent_cnt: number = 0;
  great_cnt: number = 0;
  good_cnt: number = 0;
  bad_cnt: number = 0;
  missing_cnt: number = 0;
  dubbingScore: number = 0;
  isListen: boolean = false;

  public stringURL: string;
  public static isPlaying: boolean = false;
  private isAlertFloating: boolean = false;

  // ── 리팩토링된 분할 더빙 전담 객체들 ────────────────────────────────────
  private splitCtrl: SplitDubbingController;
  private hlRenderer: ScriptHighlightRenderer;
  private buttonStart: Button = null;

  // 점수 집계 (전체 더빙 호환용)
  private points: number[] = [];
  private sumPoints: number = 0;
  private averageScore: number = 0;

  private scriptRAW: string = "";


  // ════════════════════════════════════════════════════════════════════════════
  // 초기화
  // ════════════════════════════════════════════════════════════════════════════

  start() {
    this.script = new Dubbing_Script();
    this.paragraphLabel.string = I18n.inst.t("contents_L004_P-Paragraph");

    const launcher = find("Launcher");
    this.audioSource = launcher.getComponent(AudioSource);

    if (infos.actData.type === "Practice") {
      this.plus.on("click", this.onClickFontBigger.bind(this));
      this.minus.on("click", this.onClickFontSmaller.bind(this));
      this.onOffBtn.on("click", this.onScript.bind(this));
      this.onOffScript = this.onOffLabel.getComponent(Label);

      addEventListener("visibilitychange", () => {
        if (!this.isAlertFloating && document.visibilityState === "hidden") {
          alert("녹음 도중에 창을 최소화하면 녹음 결과에 문제가 생길 수 있습니다");
          this.isAlertFloating = true;
        }
      });
    }

    // 전체 더빙 버튼 이벤트
    this.dubbingStart.parent.on("click", () => {
      Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
      this.onClickStart();
    });
    this.dubbingStop.on("click", () => {
      Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
      this.onClickStop();
    });
    this.dubbingStop.getComponent(Button).onDisable();

    // Try 번호 표시
    if (this.tryNumber) this.tryNumber.getComponent(Label).string = `Try no.${infos.tryCount}`;
    if (this.tryNumber2) this.tryNumber2.getComponent(Label).string = `Try no.${infos.tryCount}`;

    // 결과 버튼
    this.resultBtn = this.resultCon.getChildByName("resultBtn");
    this.resultBtn.on("click", () => {
      this.resultBtn.getComponent(Button).onDisable();
      this.onClickResultBtn();
    });
    this.resultBtn.getComponent(Button).onDisable();

    this.listen.on("click", this.onClickListenBtn.bind(this));

    infos.learningTime = new Date().getTime();

    this.video = this.node.getComponent(VP);
    this.record = this.dubbingStart.getComponent(LearningRecord);

    this.bindVideoEvent(false);
    this.record.SetEndEvent(this.RecordComplete.bind(this));

    this.scene = director.getScene().name;
    this.DataLoad();
  }

  DataLoad(callback?: () => void) {
    this.act_id = Utill.getQueryStringValue("act_id");
    this.gbn = Utill.getQueryStringValue("act_gbn");
    this.GetVoiceLevel();

    WebAPI.Score("L004", this.gbn, (temp: any) => {
      const data: Res_Form.Scroe_L004 = temp;
      this.weight = infos.actData.type === "Practice" ? data.practice : data.test;
    });

    infos.currentQuestionIdx = 0;
    infos.actData.domain = infos.learningData.result.act_asset_domain;
    infos.actData.skill = infos.learningData.result.act_asset_skill;
    infos.actData.type = infos.learningData.result.act_asset_type;

    this.SetData();
    if (this.senData.video_uri) this.video.SetVideoData(this.senData.video_uri);

    const video = document.getElementsByClassName("cocosVideo")[0] as HTMLVideoElement;
    video.style.zIndex = "3";
    const canvas = document.getElementById("GameCanvas");
    canvas.style.position = "relative";
    canvas.style.zIndex = "4";

    if (infos.tryCount === 1) WebAPI.Progress_Start(this.act_id, this.gbn);

    this.initSplitDubbing();
  }

  SetData() {
    this.script.CreateScript_Str();
    this.senData = infos.learningData.data[infos.currentQuestionIdx];
    this.isComplete = false;

    this.changeStar();

    if (infos.actData.type === "Practice") {
      this.resultText.getComponent(Label).string = this.script.GetStr_CompleteScript();
      this.starText.getComponent(Label).string = this.stars;
    }
  }


  // ════════════════════════════════════════════════════════════════════════════
  // 분할 더빙 초기화  ★ 핵심 변경 ★
  // ════════════════════════════════════════════════════════════════════════════

  private initSplitDubbing(): void {
    this.buttonStart = this.dubbingStart.parent.getComponent(Button);

    // ScriptHighlightRenderer 생성
    this.hlRenderer = new ScriptHighlightRenderer(this.scriptHighlight);

    // SplitDubbingController 생성 — 콜백으로 LearningDubbing에 결과를 돌려보냄
    this.splitCtrl = new SplitDubbingController(
      this.video,
      this.record,
      {
        onSegmentComplete: this.onSplitSegmentComplete.bind(this),
        onAllSegmentsComplete: this.onAllSplitSegmentsComplete.bind(this),
        onToggleRecordingMark: this.toggleShowRecordingMark.bind(this),
        onHideRecordingMark: () => { this.onAirSp.active = false; },
      },
    );

    this.splitCtrl.setup(this.senData.str_eng, this.senData.video_uri);
    this.hlRenderer.setup(
      Array.from({ length: this.splitCtrl.segmentCount }, (_, i) => this.splitCtrl.getScript(i)),
    );

    if (infos.actData.type === "Practice") {
      this.dubbingModeBtn.on("click", this.onClickDubbingMode.bind(this));
      this.hlRenderer.update(-1);   // 초기: 모두 흰색
    }
  }


  // ════════════════════════════════════════════════════════════════════════════
  // 분할 더빙 버튼 이벤트  ★ 핵심 변경 ★
  // ════════════════════════════════════════════════════════════════════════════

  /** 모드 토글 버튼 */
  onClickDubbingMode(): void {
    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
    if (this.splitCtrl.locked) return;

    const isNowSplit = this.splitCtrl.toggle();

    // UI 반영
    this.dubbingModeBtn.setPosition(isNowSplit ? 30 : -25, 0, 0);
    this.dubbingModeText.string = isNowSplit ? "ON" : "OFF";

    // 버튼 이벤트 / 영상 이벤트 교체
    this.bindStartButton(isNowSplit);
    this.bindStopButton(isNowSplit);
    this.bindVideoEvent(isNowSplit);
    this.record.SetEndEvent(
      isNowSplit
        ? (score: number, ws?: string[]) => this.onSplitSegmentComplete(score, ws)
        : this.RecordComplete.bind(this),
    );
  }

  /** 분할 더빙 시작 */
  private onClickStartSplitDubbing(): void {
    this.buttonStart.onDisable();
    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));

    const COUNTDOWN = 3;
    this.countStart(COUNTDOWN);
    this.splitCtrl.startSegment(COUNTDOWN);
    this.hlRenderer.update(this.splitCtrl.currentSegmentIdx);
  }

  /** 분할 더빙 수동 정지 */
  private onClickStopSplitDubbing(): void {
    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
    this.splitCtrl.stopSegmentManually();
  }

  // 버튼 바인딩 헬퍼 — 기존 on 해제 후 재등록
  private bindStartButton(isSplit: boolean): void {
    const parent = this.dubbingStart.parent;
    parent.off("click");
    parent.on("click", isSplit
      ? this.onClickStartSplitDubbing.bind(this)
      : () => { Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF)); this.onClickStart(); },
    );
  }

  private bindStopButton(isSplit: boolean): void {
    this.dubbingStop.off("click");
    this.dubbingStop.on("click", isSplit
      ? this.onClickStopSplitDubbing.bind(this)
      : () => { Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF)); this.onClickStop(); },
    );
  }


  // ════════════════════════════════════════════════════════════════════════════
  // update()  ★ 핵심 변경 ★  — 매 프레임 딱 한 줄
  // ════════════════════════════════════════════════════════════════════════════

  update(): void {
    if (!this.splitCtrl?.active) return;

    const ended = this.splitCtrl.checkSegmentEnd(this.video.player.currentTime);
    if (ended) {
      this.dubbingStop.getComponent(Button).onDisable();
      this.buttonStart.onEnable();
    }
  }


  // ════════════════════════════════════════════════════════════════════════════
  // 분할 더빙 콜백  ★ 핵심 변경 ★
  // CompleteSplitDubbingTask + readyNextSplitDubbing + endSplitDubbing 를 통합
  // ════════════════════════════════════════════════════════════════════════════

  /** 문단 1개 녹음 완료 시 호출 (record.SetEndEvent 콜백) */
  private onSplitSegmentComplete(totalscore: number, wordScore?: string[]): void {
    this.onAirSp.active = false;

    const seg = this.splitCtrl.currentScript;
    const isLast = this.splitCtrl.isLastSegment;

    // 결과 텍스트 생성
    let scoreStr: string;
    if (wordScore?.length) {
      scoreStr = ScriptHighlightRenderer.buildScoreString(
        seg.GetScript(),
        wordScore.map(Number),
        grade => {
          if (grade === "excellent") this.excellent_cnt++;
          else if (grade === "great") this.great_cnt++;
          else if (grade === "good") this.good_cnt++;
          else if (grade === "bad") this.bad_cnt++;
          else this.missing_cnt++;
        },
        isLast,
      );
    } else {
      scoreStr = `<color=#ffffff>${seg.GetStr_CompleteScript()}</color>`;
      this.missing_cnt += seg.GetWordCount();
    }

    this.dubbingText.getComponent(RichText).string += scoreStr;
    this.dubbingScore = Math.round(
      ((this.excellent_cnt + this.great_cnt + this.good_cnt) / this.script.GetWordCount()) * 100,
    );

    // 점수 누적 및 포인트 결정
    const avg = this.splitCtrl.accumulateScore(totalscore);
    this.averageScore = avg;
    infos.point = this.splitCtrl.resolvePoint(avg, this.weight);
    this.points.push(infos.point);
    this.sumPoints += infos.point;

    this.isComplete = true;

    // 하이라이트 갱신 (다음 문단 없으면 모두 OFF)
    this.hlRenderer.update(isLast ? -1 : this.splitCtrl.currentSegmentIdx + 1);
  }

  /** 모든 문단 완료 시 호출 */
  private onAllSplitSegmentsComplete(mergedBlob: Blob): void {
    console.warn("*************");
    this.deployAudioBlobURL(mergedBlob);
    this.SendSplitResult();

    // 결과 화면 표시
    this.video.ResetAndStop();
    this.onClickStop();
    this.scriptCon.active = false;
    this.resultCon.active = true;
    this.buttonStart.onEnable();

    const audio = this.record.GetAudio();
    audio.src = this.stringURL;
    audio.play();

    if (infos.actData.type === "Test") {
      this.node.getChildByPath("Layer/Mask/videoBoxIn").active = true;
      this.node.getChildByPath("Layer/Mask/videoBoxIn_No_Nav").active = false;
    }
  }


  // ════════════════════════════════════════════════════════════════════════════
  // 전체 더빙 (기존 로직 유지)
  // ════════════════════════════════════════════════════════════════════════════

  onClickStart(): void {
    this.countStart(3);
    this.dubbingStart.parent.getComponent(Button).onDisable();
    setTimeout(() => {
      this.video.SetMuteMode(false);
      this.video.ResetAndStop();
      this.video.onPlay();
      this.video.SetDubbing(true);
      this.record.SetActiveRecord();
      this.dubbingStop.getComponent(Button).onEnable();
      this.record.Dubbing(this.script.GetStr_WordScript());
      this.flashOnAir();
    }, 4000);
  }

  onClickStop(): void {
    if (!this.record.EndDubbingRecord()) return;
    this.video.ResetCurrentTime();
    this.video.onPause();
    this.video.SetDubbing(false);
    this.unschedule(this.toggleShowRecordingMark);
    this.scriptCon.active = false;
    this.resultCon.active = true;
    this.video.SetMuteMode(false);
    this.dubbingStart.active = false;
    this.dubbingStop.active = false;

    if (infos.actData.type === "Test") {
      this.node.getChildByPath("Layer/Mask/videoBoxIn").active = true;
      this.node.getChildByPath("Layer/Mask/videoBoxIn_No_Nav").active = false;
    }
  }

  RecordComplete(totalscore: number, wordScore?: string[]): void {
    this.onAirSp.active = false;

    const scriptArr = this.script.GetScript();
    const scriptStr = this.script.GetStr_CompleteScript();
    const scriptWordCount = this.script.GetWordCount();

    let scoreStr: string;
    if (wordScore?.length) {
      scoreStr = ScriptHighlightRenderer.buildScoreString(
        scriptArr,
        wordScore.map(Number),
        grade => {
          if (grade === "excellent") this.excellent_cnt++;
          else if (grade === "great") this.great_cnt++;
          else if (grade === "good") this.good_cnt++;
          else if (grade === "bad") this.bad_cnt++;
          else this.missing_cnt++;
        },
      );
    } else {
      scoreStr = `<color=#ffffff>${this.getColorScriptFallback(scriptArr)}</color>`;
      this.missing_cnt += scriptWordCount;
    }

    this.dubbingText.getComponent(RichText).string += scoreStr;
    this.dubbingScore = Math.round(
      ((this.excellent_cnt + this.great_cnt + this.good_cnt) / this.script.GetWordCount()) * 100,
    );

    this.points.push(infos.point);
    infos.point = this.splitCtrl.resolvePoint(totalscore, this.weight);
    this.isComplete = true;
    this.SendResult();
  }

  private getColorScriptFallback(scriptArr: { word: boolean; str: string }[]): string {
    return scriptArr.map(e => e.str).join("");
  }


  // ════════════════════════════════════════════════════════════════════════════
  // 이벤트 바인딩
  // ════════════════════════════════════════════════════════════════════════════

  private bindVideoEvent(isSplit: boolean): void {
    if (isSplit) {
      this.video.SetPauseEvent(this.OnPause.bind(this));
      this.video.SetPlayEvent(() => { });
      this.video.SetEndEvent(() => { });          // 분할: update()가 감시
    } else {
      this.video.SetPauseEvent(this.OnPause.bind(this));
      this.video.SetPlayEvent(this.OnPlay.bind(this));
      this.video.SetVolumeEvent(this.OnVolume.bind(this));
      this.video.SetProgressEvent(this.OnProgress.bind(this));
      this.video.SetEndEvent(() => this.onClickStop());
    }
  }


  // ════════════════════════════════════════════════════════════════════════════
  // 폰트 조절 (ScriptHighlightRenderer 위임)
  // ════════════════════════════════════════════════════════════════════════════

  private onClickFontBigger(): void {
    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
    this.hlRenderer.increaseFontSize();
  }

  private onClickFontSmaller(): void {
    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
    this.hlRenderer.decreaseFontSize();
  }


  // ════════════════════════════════════════════════════════════════════════════
  // 나머지 기존 메서드 (변경 없음)
  // ════════════════════════════════════════════════════════════════════════════

  OnPause() { if (this.record.GetAudio() && this.isListen) this.record.GetAudio().pause(); }
  OnPlay() { if (this.record.GetAudio() && this.isListen) this.record.GetAudio().play(); }
  OnVolume(size: number) { if (this.record.GetAudio()) this.record.GetAudio().volume = size; }
  OnProgress(time: number) {
    const audio = this.record.GetAudio();
    if (!audio) return;
    if (this.record.GetDuration() < time) { audio.currentTime = 0; audio.pause(); }
    else { audio.currentTime = time; audio.play(); }
  }

  onClickListenBtn(): void {
    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
    const nextProcess = () => {
      if (infos.learningData.data[infos.currentQuestionIdx].audio_uri) {
        SoundManager.PlaySound({ url: infos.learningData.data[infos.currentQuestionIdx].audio_uri, volume: 1 });
      }
      this.isListen = true;
      this.video.ResetCurrentTime();
      this.video.onPlay();
      this.video.onDubbingMute();
      this.video.IsPlaying(true);
      this.video.SetListen();
      this.record.GetAudio().currentTime = 0;
      this.record.GetAudio().onended = () => { this.isComplete = true; };
    };
    this.record.PlayMyVoice(nextProcess);
  }

  onClickResultBtn(): void {
    infos.dubbingResults.push({
      excellent_cnt: this.excellent_cnt,
      great_cnt: this.great_cnt,
      good_cnt: this.good_cnt,
      bad_cnt: this.bad_cnt,
      missing_cnt: this.missing_cnt,
      totalWords: this.script.GetWordCount(),
    });
    if (!this.isComplete) return;

    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
    this.record.GetAudio().pause();

    if (!this.splitCtrl?.active) {
      this.deployAudioBlobURL(infos.voiceCheck.blob);
    }
    director.loadScene(`Dubbing_${infos.actData.type}_Result`);
  }

  onScript(): void {
    Postie.Publish("SoundPlay", new SoundPlay(sound.clickEF));
    if (this.starScript) {
      this.onOffScript.string = "ON";
      this.starText.active = false;
      this.scriptHighlight.node.active = true;
      this.starScript = false;
      this.onOffBtn.setPosition(30, 0, 0);
    } else {
      this.onOffScript.string = "OFF";
      this.starText.active = true;
      this.scriptHighlight.node.active = false;
      this.starScript = true;
      this.onOffBtn.setPosition(-25, 0, 0);
    }
  }

  changeStar(): void {
    this.scriptRAW = this.senData.str_eng
      .replace(/\r\n/g, "\n")
      .replace(/\[\[\@\@\]\]/g, "");
    this.stars = this.scriptRAW.replace(/[a-zA-Z]/g, "*");
  }

  toggleShowRecordingMark(): void {
    this.isOnAir = !this.isOnAir;
    this.onAirSp.active = this.isOnAir;
  }

  flashOnAir(): void {
    if (this.record.GetState() === Record.E_STATE.NORMAL) {
      this.unschedule(this.toggleShowRecordingMark);
    } else {
      this.schedule(this.toggleShowRecordingMark, 0.5);
    }
  }

  countStart(seconds: number): void {
    const strShow = ["Start!", ...Array.from({ length: seconds }, (_, i) => (seconds - i).toString())];
    let idx = 0;
    const countLabel = this.countNode.getComponent(Label);
    const anim = this.countNode.getComponent(Animation);

    this.countNode.active = true;
    countLabel.string = strShow[idx++];
    anim.play("Ani_Dubbing_Count_Start");

    const interval = setInterval(() => {
      countLabel.string = strShow[idx++];
      anim.play("Ani_Dubbing_Count_Start");
      if (idx > strShow.length) clearInterval(interval);
    }, 1000);

    setTimeout(() => {
      countLabel.string = "";
      this.countNode.active = false;
    }, (seconds + 1) * 1000);
  }

  CreateText(): Node {
    const node = instantiate(this.dubbingText);
    const content = this.resultCon.getChildByPath("script/ScrollView/view/content");
    content.addChild(node);
    const rich = node.getComponent(RichText);
    rich.string = "";
    node.getComponent(UITransform).setAnchorPoint(0, 0.5);
    return node;
  }

  deployAudioBlobURL(audioBlob: Blob): void {
    this.stringURL = URL.createObjectURL(audioBlob);
    infos.voiceCheck.blob = audioBlob;
    infos.learningData.data[infos.currentQuestionIdx].audio_uri = this.stringURL;
    infos.localAudioFile.push(this.stringURL);
  }

  async GetVoiceLevel(): Promise<void> {
    return new Promise(resolve => {
      WebAPI.VoiceLevel((data: Res_Form.VoiceLevel) => resolve());
    });
  }

  SendResult(): void {
    WebAPI.Progress_Result({
      act_id: this.act_id,
      sco_id: infos.learningData.data[infos.currentQuestionIdx].code,
      act_gbn: this.gbn,
      qst_seq: infos.currentQuestionIdx + 1,
      try_seq: infos.tryCount,
      right_answer: this.script.GetStr_WordScript(),
      excellent_cnt: this.excellent_cnt,
      great_cnt: this.great_cnt,
      good_cnt: this.good_cnt,
      bad_cnt: this.bad_cnt,
      missing_cnt: this.missing_cnt,
      dubbing_score: this.dubbingScore,
    }, infos.voiceCheck.blob, () => this.SendEnd());
  }

  SendSplitResult(): void {
    WebAPI.Progress_Result({
      act_id: this.act_id,
      sco_id: infos.learningData.data[infos.currentQuestionIdx].code,
      act_gbn: this.gbn,
      qst_seq: infos.currentQuestionIdx + 1,
      try_seq: infos.tryCount,
      right_answer: this.scriptRAW,
      excellent_cnt: this.excellent_cnt,
      great_cnt: this.great_cnt,
      good_cnt: this.good_cnt,
      bad_cnt: this.bad_cnt,
      missing_cnt: this.missing_cnt,
      dubbing_score: this.dubbingScore,
    }, infos.voiceCheck.blob, () => this.SendEnd());
  }

  SendEnd(): void {
    WebAPI.Progress_End({
      act_id: this.act_id,
      act_gbn: this.gbn,
      act_fd: infos.learningData.result.act_asset_fd_default,
      start_cnt: 3,
      act_score: this.dubbingScore,
      pass_qst_cnt: infos.learningData.data.length,
      fail_qst_cnt: 0,
      pass_try_avg: 0,
      playtime: this.record.GetDuration(),
      point: infos.point,
      feedback_yn: "N",
    }, () => { this.resultBtn.getComponent(Button).onEnable(); });
  }
}
