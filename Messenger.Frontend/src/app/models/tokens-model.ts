import { JwtTokenModel } from "./jwt-token-model";

export interface TokensModel {
    accessToken: JwtTokenModel
    refreshToken: JwtTokenModel
}
