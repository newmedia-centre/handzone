/* eslint-disable */
export interface VideoFrame {
    data:               string;
    fourCC:             number;
    frameFormatType:    number;
    frameRateD:         number;
    frameRateN:         number;
    lineStrideBytes:    number;
    pictureAspectRatio: number;
    timecode:           number[];
    timestamp:          number[];
    type:               Type;
    xres:               number;
    yres:               number;
}

export enum Type {
    Video = "video",
}
