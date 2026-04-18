import { Button, Node, RichText } from "cc";
import { VP } from "../../../video-player/VP";
import { infos } from "../../info/game/info";
import { LearningRecord } from "../../UI/Common/Learning/LearningRecord";
import { Dubbing_Script } from "./Dubbing_Script";

// ─────────────────────────────────────────────
// 타입 정의
// ─────────────────────────────────────────────
export interface SplitDubbingCallbacks {
  onSegmentComplete: (totalscore: number, wordScore?: string[]) => void;
  onAllSegmentsComplete: (mergedBlob: Blob) => void;
  onToggleRecordingMark: () => void;
  onHideRecordingMark: () => void;
}

export interface ScoreWeight {
  variable1_point: number;
  variable2_point: number;
  variable3_point: number;
  variable4_point: number;
  variable5_point: number;
}

// ─────────────────────────────────────────────
// SplitDubbingController
// 분할 더빙의 상태와 흐름을 단독으로 관리한다.
// ─────────────────────────────────────────────
export class SplitDubbingController {

  // 외부 의존성
  private video: VP;
  private record: LearningRecord;
  private callbacks: SplitDubbingCallbacks;

  // 분할 스크립트 데이터
  private segments: Dubbing_Script[] = [];          // 문단별 스크립트
  private segmentTimestamps: number[] = [];         // 각 문단의 영상 종료 시점(초)
  private currentIdx: number = -1;                  // 현재 문단 인덱스 (-1 = 비활성)

  // 상태 플래그
  private isActive: boolean = false;                // 분할 더빙 모드 활성화 여부
  private isManualStopped: boolean = false;         // 수동 정지(stop 버튼) 여부
  private isLocked: boolean = false;                // 모드 변경 잠금
  private isDubbing: boolean = false;               // 더빙 중인지 여부

  // 녹음 데이터
  private recordedBlobs: Blob[] = [];

  // 점수 집계
  private scores: number[] = [];
  private sumScores: number = 0;

  // ─────────────────────────────────────────────
  // 초기화
  // ─────────────────────────────────────────────

  constructor(
    video: VP,
    record: LearningRecord,
    callbacks: SplitDubbingCallbacks,
  ) {
    this.video = video;
    this.record = record;
    this.callbacks = callbacks;
  }

  /**
   * 원본 스크립트 문자열과 영상 URL로 분할 데이터를 설정한다.
   * @param rawScript  "[[@@]]" 구분자를 포함한 전체 스크립트
   * @param videoUrl   "t=0.0-1.30-2.60" 형태의 타임스탬프를 포함한 URL
   */
  setup(rawScript: string, videoUrl: string): void {
    this.parseSegments(rawScript);
    this.parseTimestamps(videoUrl);
    this.reset();

    // 문단이 하나뿐이면 분할 모드 사용 불가
    if (this.segments.length <= 1) {
      this.isLocked = true;
    }
  }

  private parseSegments(rawScript: string): void {
    const parts = rawScript.split("[[@@]]").filter(s => s !== "");
    const lastIdx = parts.length - 1;

    this.segments = parts.map((part, i) => {
      const gen = new Dubbing_Script();
      const text = i === lastIdx ? part.trim() : part.trim() + "\n";
      gen.CreateScript_Str(text);
      return gen;
    });
  }

  private parseTimestamps(videoUrl: string): void {
    const marker = videoUrl.indexOf("t=");
    if (marker === -1) return;

    const raw = videoUrl.substring(marker + 2).split("-");
    this.segmentTimestamps = raw.map(t => this.timeStringToSeconds(t));
  }

  private timeStringToSeconds(timeStr: string): number {
    const parts = timeStr.split(".");
    if (parts.length === 2) return Number(parts[0]) + Number(parts[1]) / 100;
    if (parts.length === 3) {
      return Number(parts[0]) * 60 + Number(parts[1]) + Number(parts[2]) / 100;
    }
    return 0;
  }

  private reset(): void {
    this.currentIdx = -1;
    this.isManualStopped = false;
    this.recordedBlobs = [];
    this.scores = [];
    this.sumScores = 0;
  }

  // ─────────────────────────────────────────────
  // 공개 상태 조회
  // ─────────────────────────────────────────────

  get active(): boolean { return this.isActive; }
  get locked(): boolean { return this.isLocked; }
  get segmentCount(): number { return this.segments.length; }
  get currentSegmentIdx(): number { return this.currentIdx; }

  /** 현재 재생 중인 문단이 마지막인지 여부 */
  get isLastSegment(): boolean {
    return this.currentIdx >= this.segmentTimestamps.length - 1;
  }

  /** 현재 문단의 영상 종료 시점(초) */
  get currentEndTime(): number {
    return this.segmentTimestamps[this.currentIdx] ?? Infinity;
  }

  // ─────────────────────────────────────────────
  // 모드 토글
  // ─────────────────────────────────────────────

  toggle(): boolean {
    if (this.isLocked) return this.isActive;
    this.isActive = !this.isActive;
    this.currentIdx = this.isActive ? 0 : -1;
    return this.isActive;
  }

  lock(): void { this.isLocked = true; }

  // ─────────────────────────────────────────────
  // 더빙 시작
  // ─────────────────────────────────────────────

  /**
   * 현재 문단 더빙을 시작한다.
   * 첫 시작이면 영상을 처음부터, 이어가는 경우 인덱스를 올린다.
   */
  startSegment(countdownSeconds: number = 3): void {
    this.lock();
    this.isManualStopped = false;
    this.isDubbing = true;

    // 첫 시작 vs 이어가기
    const isFirstSegment = this.record.GetTimeRecordStart() == null;
    if (isFirstSegment) {
      this.currentIdx = 0;
      this.video.ResetAndStop();
    } else {
      this.currentIdx++;
    }

    this.video.SetMuteMode(false);

    // 녹음은 즉시 시작 (초기 안정화 노이즈는 merge 시 잘라냄)
    this.record.SetActiveRecord();
    this.record.RecordSplitDubbing(this.currentScript.GetStr_WordScript());

    // 카운트다운 후 영상 재생
    setTimeout(() => {
      this.beginPlayback();
    }, (countdownSeconds + 1) * 1000);
  }

  private beginPlayback(): void {
    this.video.SetDubbing(true);
    this.record.SetTimeRecordStart(new Date().getTime());
    this.video.onPlay();
    this.callbacks.onToggleRecordingMark();
  }

  // ─────────────────────────────────────────────
  // update() 에서 호출: 영상 시간 감시
  // ─────────────────────────────────────────────

  /**
   * 매 프레임 호출. 현재 문단의 종료 시점에 도달하면 일시정지를 처리한다.
   * @param currentVideoTime 영상의 현재 재생 위치(초)
   * @returns 이번 프레임에 문단 종료를 처리했으면 true
   */
  checkSegmentEnd(currentVideoTime: number): boolean {
    if (!this.isDubbing) return false;
    if (!this.isActive || this.currentIdx < 0) return false;
    if (currentVideoTime < this.currentEndTime) return false;

    // 종료 시점 도달 → 일시정지 처리 후 플래그 해제
    this.handleSegmentPause();
    return true;
  }

  // ─────────────────────────────────────────────
  // 일시정지 처리 (자동 / 수동 공통)
  // ─────────────────────────────────────────────

  /**
   * 영상 시간 도달에 의한 자동 일시정지.
   */
  private handleSegmentPause(): void {
    this.isDubbing = false;
    this.video.onPause();

    if (!this.isManualStopped) {
      this.stopRecordAndPrepareNext();
    }

    this.safelyPauseAudio();
    this.video.SetMuteMode(false);
  }

  /**
   * 수동 정지 버튼에 의한 일시정지.
   */
  stopSegmentManually(): void {
    this.isManualStopped = true;
    this.callbacks.onToggleRecordingMark();
    this.stopRecordAndPrepareNext();
    this.safelyPauseAudio();
  }

  private stopRecordAndPrepareNext(): void {
    this.callbacks.onToggleRecordingMark();

    // 녹음 종료 완료 콜백을 일회성으로 연결
    this.record.eventTrigger.once("readyNextSplitDubbing", () => {
      this.onSegmentRecordingFinished();
    });

    this.record.RecordDubbingStop(true);
  }

  private safelyPauseAudio(): void {
    const audio = this.record.GetAudio();
    if (audio?.src && !audio.paused && audio.readyState >= 2) {
      try { audio.pause(); } catch { /* 무시 */ }
    } else {
      audio.autoplay = false;
      audio.preload = "none";
      console.warn(audio.readyState);
    }
  }

  // ─────────────────────────────────────────────
  // 문단 녹음 완료 처리
  // ─────────────────────────────────────────────

  private onSegmentRecordingFinished(): void {
    // Blob 저장
    this.recordedBlobs.push(infos.voiceCheck.blob);

    if (this.isLastSegment) {
      // 모든 문단 완료 → Blob 병합 후 상위에 알림
      const merged = this.mergeWavBlobs(this.recordedBlobs, 11025, 1, 3);
      this.callbacks.onAllSegmentsComplete(merged);
    }
    // 다음 문단은 start 버튼을 눌러야 시작 (자동 진행 없음)
  }

  // ─────────────────────────────────────────────
  // 점수 계산
  // ─────────────────────────────────────────────

  /**
   * 문단 완료마다 호출. 누적 평균 점수를 반환한다.
   */
  accumulateScore(segmentScore: number): number {
    this.scores.push(segmentScore);
    this.sumScores += segmentScore;
    return this.sumScores / this.scores.length;
  }

  /**
   * 누적 평균 점수를 weight 테이블에 따라 포인트로 변환한다.
   */
  resolvePoint(averageScore: number, weight: ScoreWeight): number {
    if (averageScore > 4999) return weight.variable1_point;
    if (averageScore > 2999) return weight.variable2_point;
    if (averageScore > 399) return weight.variable3_point;
    if (averageScore > 0) return weight.variable4_point;
    return weight.variable5_point;
  }

  // ─────────────────────────────────────────────
  // 스크립트 접근자
  // ─────────────────────────────────────────────

  get currentScript(): Dubbing_Script {
    return this.segments[this.currentIdx];
  }

  getScript(idx: number): Dubbing_Script {
    return this.segments[idx];
  }

  // ─────────────────────────────────────────────
  // WAV Blob 병합
  // ─────────────────────────────────────────────

  private mergeWavBlobs(
    blobs: Blob[],
    sampleRate: number,
    numChannels: number,
    cutSecond?: number,
  ): Blob {
    const HEADER_SIZE = 44;
    const cutBytes = cutSecond != null && cutSecond > 0
      ? Math.floor(sampleRate * numChannels * 2 * cutSecond)
      : 0;

    const audioChunks = blobs.map((blob, i) => {
      const skip = HEADER_SIZE + (i === 0 ? cutBytes : 0);
      return blob.slice(skip);
    });

    const totalDataLen = audioChunks.reduce((sum, c) => sum + c.size, 0);
    const header = this.buildWavHeader(totalDataLen, sampleRate, numChannels);

    return new Blob([header, ...audioChunks], { type: "audio/wav" });
  }

  private buildWavHeader(
    dataLength: number,
    sampleRate: number,
    numChannels: number,
  ): ArrayBuffer {
    const buf = new ArrayBuffer(44);
    const view = new DataView(buf);
    const write = (offset: number, str: string) =>
      [...str].forEach((c, i) => view.setUint8(offset + i, c.charCodeAt(0)));

    write(0, "RIFF");
    view.setUint32(4, dataLength, true);
    write(8, "WAVE");
    write(12, "fmt ");
    view.setUint32(16, 16, true);
    view.setUint16(20, 1, true);
    view.setUint16(22, numChannels, true);
    view.setUint32(24, sampleRate, true);
    view.setUint32(28, sampleRate * numChannels * 2, true);
    view.setUint16(32, numChannels * 2, true);
    view.setUint16(34, 16, true);
    write(36, "data");
    view.setUint32(40, dataLength, true);

    return buf;
  }
}
